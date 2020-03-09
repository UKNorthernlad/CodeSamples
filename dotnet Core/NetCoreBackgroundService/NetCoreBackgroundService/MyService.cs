using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventLogWriter
{
    public class SampleService : IHostedService
    {

        private int executionCount = 0;
        private readonly ILogger<SampleService> _logger;
        private Timer _timer;

        public SampleService(ILogger<SampleService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("StartAsync being called.......");
            // For examples of typical things to do in a background worker, see
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-3.1&tabs=visual-studio

            _timer = new Timer(DoWork, null, TimeSpan.Zero,TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var count = Interlocked.Increment(ref executionCount);

            _logger.LogInformation("Timed Hosted Service is working. Count: {Count}", count);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("StopAsync being called.......");
            //throw new System.NotImplementedException();

            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}



