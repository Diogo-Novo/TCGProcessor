using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TCGProcessor.Interfaces;

namespace TCGProcessor.Services
{
    // SignalR Hub
    public class ProcessingHub : Hub
    {
        private readonly ILogger<ProcessingHub> _logger;
        private readonly IJobTracker _jobTracker;

        public ProcessingHub(ILogger<ProcessingHub> logger, IJobTracker jobTracker)
        {
            _logger = logger;
            _jobTracker = jobTracker;
        }

        public async Task JoinJobGroup(string jobId)
        {
            try
            {
                // Validate that the job exists
                var jobStatus = _jobTracker.GetJobStatus(jobId);
                if (jobStatus == null)
                {
                    await Clients.Caller.SendAsync("Error", new { Message = $"Job {jobId} not found" });
                    return;
                }

                var jobGroup = $"job_{jobId}";
                await Groups.AddToGroupAsync(Context.ConnectionId, jobGroup);
                
                _logger.LogInformation("Client {ConnectionId} joined job group {JobGroup}", Context.ConnectionId, jobGroup);

                // Send current job status to the newly connected client
                await Clients.Caller.SendAsync("JobStatusUpdate", new
                {
                    JobId = jobId,
                    Status = jobStatus.Status,
                    Progress = jobStatus.Progress,
                    Message = jobStatus.Message,
                    Result = jobStatus.Result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining job group for job {JobId}", jobId);
                await Clients.Caller.SendAsync("Error", new { Message = "Failed to join job tracking" });
            }
        }

        public async Task LeaveJobGroup(string jobId)
        {
            try
            {
                var jobGroup = $"job_{jobId}";
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, jobGroup);
                
                _logger.LogInformation("Client {ConnectionId} left job group {JobGroup}", Context.ConnectionId, jobGroup);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error leaving job group for job {JobId}", jobId);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation("Client {ConnectionId} disconnected", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        // Optional: Allow clients to check if a job is still being processed
        public async Task CheckJobStatus(string jobId)
        {
            try
            {
                var jobStatus = _jobTracker.GetJobStatus(jobId);
                if (jobStatus == null)
                {
                    await Clients.Caller.SendAsync("JobNotFound", new { JobId = jobId });
                    return;
                }

                await Clients.Caller.SendAsync("JobStatusUpdate", new
                {
                    JobId = jobId,
                    Status = jobStatus.Status,
                    Progress = jobStatus.Progress,
                    Result = jobStatus.Result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking job status for job {JobId}", jobId);
                await Clients.Caller.SendAsync("Error", new { Message = "Failed to check job status" });
            }
        }
    }
}