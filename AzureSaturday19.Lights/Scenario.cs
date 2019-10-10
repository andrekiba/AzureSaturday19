using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using AzureSaturday19.Lights.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureSaturday19.Lights
{
    public static class Scenario
    {
        [FunctionName("ScenarioOrchestrator")]
        public static async Task<bool> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            try
            {
                var scenario = context.GetInput<ScenarioRequest>();
                var lights = scenario.LightRequests.Select(lr => new EntityId(nameof(Light), lr.LightId)).ToArray();

                #region Old

                //var light1 = new EntityId(nameof(Light), "hue1");
                //var light2 = new EntityId(nameof(Light), "hue2");

                //using (await context.LockAsync(light1, light2))
                //{
                //	var light1Proxy = context.CreateEntityProxy<ILight>(light1);
                //	var light2Proxy = context.CreateEntityProxy<ILight>(light2);

                //	var ligth1State = await light1Proxy.Get();
                //	var ligth2State = await light2Proxy.Get();

                //	switch (ligth1State)
                //	{
                //		case LightState.On when ligth2State == LightState.Off:
                //			light1Proxy.Off();
                //			light2Proxy.On();
                //			break;
                //		case LightState.Off when ligth2State == LightState.On:
                //			light1Proxy.On();
                //			light2Proxy.Off();
                //			break;
                //		default:
                //			light1Proxy.Off();
                //			light2Proxy.Off();
                //			break;
                //	}
                //}

                #endregion

                using (await context.LockAsync(lights))
                {
                    for (var i = 0; i < lights.Length; i++)
                    {
                        var lightProxy = context.CreateEntityProxy<ILight>(lights[i]);
                        var lightRequest = scenario.LightRequests[i];
                        //var prevState = await context.CallEntityAsync<LightState>(lights[i], "Get");
                        switch (lightRequest.LightAction)
                        {
                            case LightAction.Off:
                                await context.CallEntityAsync(lights[i], "Off");
                                //context.SignalEntity(lights[i], "Off");
                                //lightProxy.Off();
                                break;
                            case LightAction.On:
                                await context.CallEntityAsync(lights[i], "On");
                                //lightProxy.On();
                                break;
                            case LightAction.Color:
                                await context.CallEntityAsync(lights[i], "On");
                                await context.CallEntityAsync(lights[i], "Color", lightRequest.HexColor);
                                //lightProxy.Color(lightRequest.HexColor);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
                return false;
            }
        }

        [FunctionName("ScenarioTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "scenario")]HttpRequest req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var scenario = JsonConvert.DeserializeObject<ScenarioRequest>(requestBody);

                var orchestratorId = await starter.StartNewAsync("ScenarioOrchestrator", scenario);

                log.LogInformation($"Started scenario with ID = '{orchestratorId}'.");

                return starter.CreateCheckStatusResponse(req, orchestratorId);
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
                return new ExceptionResult(e, true);
            }
        }
    }
}