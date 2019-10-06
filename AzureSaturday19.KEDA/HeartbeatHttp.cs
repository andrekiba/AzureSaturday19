using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;

namespace AzureSaturday19.KEDA
{
    public class HeartbeatHttp
    {
        readonly IHeartbeatService heartbeatService;

        public HeartbeatHttp(IHeartbeatService heartbeatService)
        {
            this.heartbeatService = heartbeatService;
        }

        [FunctionName("heartbeathttp")]
        public async Task<string> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req,
            CancellationToken cancellationToken)
            => await heartbeatService.Heartbeat(cancellationToken);
    }
}
