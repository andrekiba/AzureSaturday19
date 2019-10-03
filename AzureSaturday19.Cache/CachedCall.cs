using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AzureSaturday19.Cache
{
    public static class CachedCall
	{
        [FunctionName("CachedCall")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "data/{userId}")] HttpRequest req,
			string userId,
			[DurableClient] IDurableClient client,
            ILogger log)
        {
			log.LogInformation("C# HTTP trigger function processed a request.");

			var cacheId = new EntityId(nameof(ByteCache), $"cache{userId}");
			var state = await client.ReadEntityStateAsync<byte[]>(cacheId);		

			if (state.EntityExists)
				return new OkObjectResult(state.EntityState);
			else
			{
				//simulate call to external service to retrieve data
				await Task.Delay(TimeSpan.FromSeconds(2));
				var data = new byte[] { 0x60, 0x60 };
				
				await client.SignalEntityAsync<ICache<byte[]>>(cacheId, proxy => proxy.Set(data));

				var orchestratorInstanceId = await client.StartNewAsync(nameof(CacheOrchestrator), cacheId);

				return new OkObjectResult(data);
			}
        }
    }
}
