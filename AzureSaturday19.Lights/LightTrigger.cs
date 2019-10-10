using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using AzureSaturday19.Lights.Models;

namespace AzureSaturday19.Lights
{
    public class LightTrigger
    {
        [FunctionName("LightTrigger")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "lights/{lightKey}")] HttpRequest req,
            string lightKey,
            [DurableClient] IDurableEntityClient client,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var lightRequest = JsonConvert.DeserializeObject<LightRequest>(requestBody);

                var entityId = new EntityId(nameof(Light), lightKey);

				//EntityStateResponse
                var esr = await client.ReadEntityStateAsync<Light>(entityId);
                if (esr.EntityExists)
                {
					//sto modificando lo snapshot non la entity!!
					//https://github.com/Azure/azure-functions-durable-extension/issues/960
					esr.EntityState.On();
	                var test = await esr.EntityState.Get();
				}
                
				//se voglio modificare la entity devo usare Signal
                await client.SignalEntityAsync(entityId, lightRequest.LightAction.ToString(),
                    lightRequest.LightAction == LightAction.Color ? lightRequest.HexColor : null);

                return new AcceptedResult();
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
                return new ExceptionResult(e, true);
            }
        }
    }
}
