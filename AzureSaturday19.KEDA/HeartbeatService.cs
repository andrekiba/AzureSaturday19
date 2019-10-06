using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Extensions.Logging;

namespace AzureSaturday19.KEDA
{
    public class HeartbeatService : IHeartbeatService
    {
        readonly ILogger<HeartbeatService> logger;
        readonly IHostIdProvider hostIdProvider;

        public HeartbeatService(ILogger<HeartbeatService> logger, IHostIdProvider hostIdProvider)
        {
            this.logger = logger;
            this.hostIdProvider = hostIdProvider;
        }

        public async Task<string> Heartbeat(CancellationToken cancellationToken)
        {
            logger.LogInformation("Heartbeat invoked.");

            var hostId = await hostIdProvider.GetHostIdAsync(cancellationToken);
            var when = DateTime.UtcNow.ToString("G");
            return $"I'm alive from host {hostId} at {when}!";
        }
    }
}
