using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace AzureSaturday19.Cache
{
	public static class CacheOrchestrator
    {
        [FunctionName("CacheOrchestrator")]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
			ILogger logger)
        {
			logger.LogInformation("Starting cache manager");
			
			var cacheId = context.GetInput<EntityId>();
			await context.CreateTimer(context.CurrentUtcDateTime.AddMinutes(1), CancellationToken.None);

			logger.LogInformation($"Cleaning {cacheId.EntityKey}");

			//await context.CallEntityAsync<ICache<byte[]>>(cacheId, "Clear");
			context.SignalEntity(cacheId, "Clear");
        }


		[FunctionName("CacheOrchestratorStarter")]
		public static async Task<HttpResponseMessage> HttpStart(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
			[DurableClient] IDurableOrchestrationClient starter,
			ILogger log)
		{
			// Function input comes from the request content.
			var instanceId = await starter.StartNewAsync(nameof(CacheOrchestrator), null);

			log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

			return starter.CreateCheckStatusResponse(req, instanceId);
		}
	}


}