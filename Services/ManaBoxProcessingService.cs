using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TCGProcessor.Interfaces;
using TCGProcessor.Models;

namespace TCGProcessor.Services
{
    public class ManaBoxProcessingService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<ManaBoxProcessingService> _logger;

        public ManaBoxProcessingService(
            IServiceScopeFactory scopeFactory,
            IBackgroundTaskQueue taskQueue,
            ILogger<ManaBoxProcessingService> logger
        )
        {
            _scopeFactory = scopeFactory;
            _taskQueue = taskQueue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ManaBoxProcessingService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                if (_taskQueue.TryDequeue(out var request))
                {
                    IServiceScope? scope = null;
                    try
                    {
                        scope = _scopeFactory.CreateScope();

                        var processingService =
                            scope.ServiceProvider.GetRequiredService<IJsonProcessingService>();
                        var hubContext = scope.ServiceProvider.GetRequiredService<
                            IHubContext<ProcessingHub>
                        >();
                        var jobTracker = scope.ServiceProvider.GetRequiredService<IJobTracker>();

                        var userGroup = $"job_{request.JobId}";

                        _logger.LogInformation(
                            "Starting to process job {JobId} with {CardCount} cards",
                            request.JobId,
                            request.Cards.Count
                        );

                        // Send processing started event
                        await hubContext
                            .Clients.Group(userGroup)
                            .SendAsync(
                                "ProcessingStarted",
                                new
                                {
                                    JobId = request.JobId,
                                    StartTime = DateTime.UtcNow,
                                    TotalCards = request.Cards.Count
                                },
                                stoppingToken
                            );

                        // Update job tracker
                        jobTracker.UpdateProgress(request.JobId, 5, "Starting card enrichment...");

                        // Step 1: Enrich cards with proper progress callback
                        var enrichedCards = await processingService.EnrichWithScryfallData(
                            request.Cards,
                            async progress =>
                            {
                                var progressPercent = 10 + (progress.Percentage * 0.7); // 10% to 80%
                                var status =
                                    $"Processing card {progress.Current} of {progress.Total}";

                                _logger.LogDebug(
                                    "Progress update: {Progress}% - {Status}",
                                    progressPercent,
                                    status
                                );

                                jobTracker.UpdateProgress(
                                    request.JobId,
                                    (int)progressPercent,
                                    status
                                );

                                await hubContext
                                    .Clients.Group(userGroup)
                                    .SendAsync(
                                        "ProgressUpdate",
                                        new
                                        {
                                            JobId = request.JobId,
                                            Progress = progressPercent,
                                            Status = status,
                                            CurrentPhase = "scryfall_enrichment",
                                            ProcessedItems = progress.Current,
                                            TotalItems = progress.Total
                                        },
                                        stoppingToken
                                    );
                            }
                        );

                        _logger.LogInformation(
                            "Enrichment completed for job {JobId}",
                            request.JobId
                        );

                        // Step 2: Generate pricing sheet
                        jobTracker.UpdateProgress(request.JobId, 85, "Generating pricing sheet...");
                        await hubContext
                            .Clients.Group(userGroup)
                            .SendAsync(
                                "ProgressUpdate",
                                new
                                {
                                    JobId = request.JobId,
                                    Progress = 85,
                                    Status = "Generating pricing sheet...",
                                    CurrentPhase = "pricing_calculation"
                                },
                                stoppingToken
                            );

                        var pricingSheet = await processingService.GenerateManaBoxPricingSheetItems(
                            enrichedCards,
                            request
                        );

                        // Step 3: Finalize
                        jobTracker.UpdateProgress(request.JobId, 95, "Finalizing results...");
                        await hubContext
                            .Clients.Group(userGroup)
                            .SendAsync(
                                "ProgressUpdate",
                                new
                                {
                                    JobId = request.JobId,
                                    Progress = 95,
                                    Status = "Finalizing results...",
                                    CurrentPhase = "finalization"
                                },
                                stoppingToken
                            );

                        // Complete the job
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

                        jobTracker.CompleteJob(request.JobId, result);

                        _logger.LogInformation(
                            "Sending completion event for job {JobId}",
                            request.JobId
                        );

                        await hubContext
                            .Clients.Group(userGroup)
                            .SendAsync(
                                "ProcessingCompleted",
                                new
                                {
                                    JobId = request.JobId,
                                    Success = true,
                                    Message = "Processing completed successfully",
                                    CompletedAt = DateTime.UtcNow,
                                    Stats = new
                                    {
                                        TotalCards = request.Cards.Count,
                                        ProcessedCards = enrichedCards.Count,
                                        FoundCards = enrichedCards.Count(c =>
                                            c.ScryfallData != null
                                        ),
                                        NotFoundCards = enrichedCards.Count(c =>
                                            c.ScryfallData == null
                                        ),
                                        ProcessingTimeMs = (
                                            (jobTracker.GetJobStatus(request.JobId)?.StartTime ?? DateTime.UtcNow) - DateTime.UtcNow
                                        ).TotalMilliseconds
                                    },
                                    Results = new
                                    {
                                        PricingSheetId = request.PricingSheetId,
                                        TotalValue = pricingSheet.PsTotalSellValue,
                                        ItemCount = pricingSheet.PsItemCount
                                    }
                                },
                                stoppingToken
                            );

                        _logger.LogInformation("Job {JobId} completed successfully", request.JobId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing job {JobId}", request.JobId);

                        var userGroup = $"job_{request.JobId}";

                        // Update job tracker
                        var jobTracker = scope?.ServiceProvider.GetRequiredService<IJobTracker>();
                        jobTracker?.FailJob(request.JobId, ex.Message);

                        // Send failure event
                        var hubContext = scope?.ServiceProvider.GetRequiredService<
                            IHubContext<ProcessingHub>
                        >();
                        if (hubContext != null)
                        {
                            await hubContext
                                .Clients.Group(userGroup)
                                .SendAsync(
                                    "ProcessingFailed",
                                    new
                                    {
                                        JobId = request.JobId,
                                        Success = false,
                                        Error = ex.Message,
                                        ErrorCode = "PROCESSING_ERROR",
                                        FailedAt = DateTime.UtcNow
                                    },
                                    stoppingToken
                                );
                        }
                    }
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
