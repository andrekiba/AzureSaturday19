using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;

namespace AzureSaturday19.KEDA
{
    public class HeartbeatFunction
	{
		readonly IHeartbeatService heartbeatService;

		public HeartbeatFunction(IHeartbeatService heartbeatService)
		{
			this.heartbeatService = heartbeatService;
		}

		[FunctionName(nameof(HeartbeatFunction))]
		public async Task<string> Run(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req,
			CancellationToken cancellationToken)
			=> await heartbeatService.Heartbeat(cancellationToken);
	}
}
