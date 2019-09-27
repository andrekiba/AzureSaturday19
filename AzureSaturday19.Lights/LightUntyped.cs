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
            if (ctx.IsNewlyConstructed)
            {
                ctx.SetState(new LightState
                {
                    State = false,
                    HexColor = "#efebd8"
                });
            }

            var currentState = ctx.GetState<LightState>();
            var input = ctx.GetInput<LightRequest>();

            switch (ctx.OperationName)
            {
                case "on":
                    currentState.State = true;
                    break;
                case "off":
                    currentState.State = false;
                    break;
                case "get":
                    ctx.Return(currentState);
                    break;
                case "color":
                    currentState.HexColor = input.HexColor;
                    break;
            }

            ctx.SetState(currentState);

            log.LogInformation(currentState.ToString());

            return Task.CompletedTask;
        }

        [FunctionName("LightUntypedTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "lights/{lightId}")] HttpRequest req,
            string lightId,
            [DurableClient] IDurableEntityClient client,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var lightRequest = JsonConvert.DeserializeObject<LightRequest>(requestBody);

            var entityId = new EntityId(nameof(LightUntyped), lightId);

            await client.SignalEntityAsync(entityId, lightRequest.LightAction.ToString().ToLower(), lightRequest);

            return new AcceptedResult();
        }
    }

    public class LightState
    {
        public bool State { get; set; }
        public string HexColor { get; set; }
        public override string ToString()
        {
            return $"Stete:{(State == true ? "on" : "off")} - Color:{HexColor}";
        }
    }

    public class LightRequest
    {
        public LightAction LightAction { get; set; }
        public string HexColor { get; set; }
    }

    public enum LightAction
    {
        On = 1,
        Off = 2,
        Get = 3,
        Color = 4
    }
}
