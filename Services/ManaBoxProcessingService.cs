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
        ILogger<ManaBoxProcessingService> logger)
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
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var processingService = scope.ServiceProvider.GetRequiredService<IJsonProcessingService>();
                    var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<ProcessingHub>>();
                    var jobTracker = scope.ServiceProvider.GetRequiredService<IJobTracker>();

                    var userGroup = $"job_{request.JobId}";

                    // Example: send start message
                    await hubContext.Clients.Group(userGroup).SendAsync("ProcessingStarted", new { request.JobId });

                    var enrichedCards = await processingService.EnrichWithScryfallData(request.Cards, progress =>
                        hubContext.Clients.Group(userGroup).SendAsync("ProgressUpdate", new
                        {
                            JobId = request.JobId,
                            Progress = 10 + (progress.Percentage * 0.7),
                            Status = $"Processing card {progress.Current} of {progress.Total}"
                        }));

                    var pricingSheet = await processingService.GenerateManaBoxPricingSheetItems(enrichedCards, request);

                    jobTracker.CompleteJob(request.JobId, new ProcessingResult
                    {
                        JobId = request.JobId,
                        Success = true,
                        Message = "Processing completed successfully"
                    });

                    await hubContext.Clients.Group(userGroup).SendAsync("ProcessingCompleted", new { request.JobId });

                    _logger.LogInformation("Job {JobId} completed successfully", request.JobId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing job {JobId}", request.JobId);
                }
            }

            await Task.Delay(1000, stoppingToken); // avoid busy loop
        }
    }
}

}