using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;

namespace QuartzClustering
{
    [DisallowConcurrentExecution]
    public class SampleJob : IJob
    {
        private readonly ILogger<SampleJob> _logger;
        public SampleJob(ILogger<SampleJob> logger)
        {
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Fetching sample job");

            _logger.LogInformation($"Execute job {DateTime.UtcNow}");
        }
    }

}
