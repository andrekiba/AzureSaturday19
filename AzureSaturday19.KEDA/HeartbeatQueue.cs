using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AzureSaturday19.KEDA
{
    public class HeartbeatQueue
    {
        readonly IHeartbeatService heartbeatService;

        public HeartbeatQueue(IHeartbeatService heartbeatService)
        {
            this.heartbeatService = heartbeatService;
        }

        [FunctionName("heartbeatqueue")]
        public async Task Run(
            [QueueTrigger("kedaqueue", Connection = "AzureWebJobsStorage")]string item,
            CancellationToken cancellationToken,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {item}");
            await Task.Delay(TimeSpan.FromSeconds(3));
            await heartbeatService.Heartbeat(cancellationToken);
        }
    }
}
