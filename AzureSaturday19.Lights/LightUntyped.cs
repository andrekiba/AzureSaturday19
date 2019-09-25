using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureSaturday19.Lights
{
    public static class LightUntyped
    {
        [FunctionName("LightUntyped")]
        public static Task Run(
            [EntityTrigger] IDurableEntityContext ctx,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            bool currentState = ctx.GetState<bool>();

            switch (ctx.OperationName)
            {
                case "on":
                case "off":
                    bool input = ctx.GetInput<bool>();
                    currentState = input;
                    break;
                case "get":
                    ctx.Return(currentState);
                    break;
            }

            ctx.SetState(currentState);

            log.LogInformation(currentState.ToString());

            return Task.CompletedTask;
        }

        [FunctionName("LightUntypedTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
