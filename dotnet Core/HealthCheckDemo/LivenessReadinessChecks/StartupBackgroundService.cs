using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace HealthCheckDemo
{
    // This class represents some work you want doing in the background.
    // By creating an instance and passing in the reference to the StartupHealthCheck object it can update it when the background service has done its work.
    public class StartupBackgroundService : BackgroundService
    {
        private readonly StartupHealthCheck _healthCheck;

        public StartupBackgroundService(StartupHealthCheck healthCheck)
            => _healthCheck = healthCheck;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Simulate the effect of a long-running task.
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            _healthCheck.StartupCompleted = true;
        }
    }
}
