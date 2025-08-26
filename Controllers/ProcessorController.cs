using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TCGProcessor.Data;
using TCGProcessor.Interfaces;
using TCGProcessor.Models;
using TCGProcessor.Repositories;
using TCGProcessor.Services;

namespace TCGProcessor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProcessorController : ControllerBase
    {
        #region Local Variables
        private readonly IJobTracker _jobTracker;
        private readonly IJsonProcessingService _processingService;
        private readonly ILogger<ProcessorController> _logger;
        private readonly IHubContext<ProcessingHub> _hubContext;
        private readonly IBackgroundTaskQueue _taskQueue;
        private PricingSheetRepository _pricingSystemRepository;
        #endregion
        public ProcessorController(
            ILogger<ProcessorController> logger,
            IJobTracker jobTracker,
            IJsonProcessingService processingService,
            IHubContext<ProcessingHub> hubContext,
            PricingSheetRepository pricingSystemRepository,
            IBackgroundTaskQueue taskQueue
        )
        {
            _logger = logger;
            _jobTracker = jobTracker;
            _pricingSystemRepository = pricingSystemRepository;
            _processingService = processingService;
            _hubContext = hubContext;
            _taskQueue = taskQueue;
        }

        [HttpGet("pricing")]
        public IActionResult GetPricingData()
        {
            var items = _pricingSystemRepository.GetItems();
            return Ok(items);
        }

        [HttpPost("process-manabox")]
        public async Task<IActionResult> ProcessManaBox([FromBody] ManaBoxProcessingRequest request)
        {
            try
            {
                //var userId = User.FindFirst("user_id")?.Value ?? User.Identity?.Name ?? "anonymous";
                bool pricingSheetExists = await _pricingSystemRepository.PricingSheetExists(
                    request.PricingSheetId
                );
                if (!pricingSheetExists)
                {
                    return BadRequest(
                        $"Pricing sheet with ID {request.PricingSheetId} does not exist."
                    );
                }

                // Validate the request
                if (request.Cards == null || !request.Cards.Any())
                    return BadRequest("No cards provided");

                // Validate card data
                var validation = await _processingService.ValidateCards(request.Cards);
                if (!validation.IsValid)
                {
                    return BadRequest(
                        new { Errors = validation.Errors, Warnings = validation.Warnings }
                    );
                }

                // Generate unique job ID
                var jobId = Guid.NewGuid().ToString();

                // Create processing request
                var processingRequest = new JsonProcessingRequest
                {
                    JobId = jobId,
                    Cards = request.Cards,
                    Config = request.Config,
                    EuroToGbpRate = request.EuroToGbpRate,
                    UsdToGbpRate = request.UsdToGbpRate,
                    PricingSheetId = request.PricingSheetId
                };

                // Start tracking the job
                _jobTracker.StartJob(jobId);

                // Start background processing
                _taskQueue.QueueJob(processingRequest);
                _logger.LogInformation(
                    "Started processing job {JobId} for user {UserId} with {CardCount} cards",
                    jobId,
                    request.Cards.Count
                );

                return Ok(
                    new
                    {
                        JobId = jobId,
                        Message = "Processing started",
                        SignalRGroup = $"job_{jobId}", // Group name for SignalR connection
                        ValidationWarnings = validation.Warnings
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting card processing");
                return StatusCode(500, $"Error starting processing: {ex.Message}");
            }
        }

        private async Task ProcessManaBoxCardsAsync(JsonProcessingRequest request)
        {
            var userGroup = $"job_{request.JobId}";

            try
            {
                await _hubContext
                    .Clients.Group(userGroup)
                    .SendAsync("ProcessingStarted", new { JobId = request.JobId });

                // Step 1: Enrich with Scryfall data
                _jobTracker.UpdateProgress(
                    request.JobId,
                    10,
                    "Fetching card data from Scryfall..."
                );
                await _hubContext
                    .Clients.Group(userGroup)
                    .SendAsync(
                        "ProgressUpdate",
                        new
                        {
                            JobId = request.JobId,
                            Progress = 10,
                            Status = "Fetching card data from Scryfall..."
                        }
                    );

                var enrichedCards = await _processingService.EnrichWithScryfallData(
                    request.Cards,
                    progress =>
                        _hubContext
                            .Clients.Group(userGroup)
                            .SendAsync(
                                "ProgressUpdate",
                                new
                                {
                                    JobId = request.JobId,
                                    Progress = 10 + (progress.Percentage * 0.7), // 10% to 80%
                                    Status = $"Processing card {progress.Current} of {progress.Total}"
                                }
                            )
                );

                // Step 2: Generate pricing sheet
                _jobTracker.UpdateProgress(request.JobId, 85, "Generating pricing sheet items...");
                await _hubContext
                    .Clients.Group(userGroup)
                    .SendAsync(
                        "ProgressUpdate",
                        new
                        {
                            JobId = request.JobId,
                            Progress = 85,
                            Status = "Generating pricing sheet..."
                        }
                    );

                var pricingSheet = await _processingService.GenerateManaBoxPricingSheetItems(
                    enrichedCards,
                    request
                );

                // Step 3: Save results (optional)
                _jobTracker.UpdateProgress(request.JobId, 95, "Finalizing results...");
                await _hubContext
                    .Clients.Group(userGroup)
                    .SendAsync(
                        "ProgressUpdate",
                        new
                        {
                            JobId = request.JobId,
                            Progress = 95,
                            Status = "Finalizing results..."
                        }
                    );

                // Complete
                var result = new ProcessingResult
                {
                    JobId = request.JobId,
                    Success = true,
                    Message = "Processing completed successfully",
                    Stats = new ProcessingStats
                    {
                        TotalCards = request.Cards.Count,
                        ProcessedCards = enrichedCards.Count,
                        FoundCards = enrichedCards.Count(c => c.ScryfallData != null),
                        NotFoundCards = enrichedCards.Count(c => c.ScryfallData == null)
                    }
                };

                _jobTracker.CompleteJob(request.JobId, result);
                await _hubContext.Clients.Group(userGroup).SendAsync("ProcessingCompleted", result);

                _logger.LogInformation(
                    "Successfully completed processing job {JobId}",
                    request.JobId
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing job {JobId}", request.JobId);

                _jobTracker.FailJob(request.JobId, ex.Message);
                await _hubContext
                    .Clients.Group(userGroup)
                    .SendAsync(
                        "ProcessingFailed",
                        new { JobId = request.JobId, Error = ex.Message }
                    );
            }
        }
    }
}
