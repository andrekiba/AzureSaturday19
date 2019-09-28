using System.IO;
using System.Threading.Tasks;
using AzureSaturday19.Lights.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AzureSaturday19.Lights
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Light
	{
		readonly ILogger log;
		public Light(ILogger log)
		{
			this.log = log;
		}

		#region Properties

		[JsonProperty]
		[JsonConverter(typeof(StringEnumConverter))]
		public LightState State { get; private set; } = LightState.Off;
		[JsonProperty]
		public string HexColor { get; private set; } = "#efebd8";
		[JsonProperty]
		public string Message { get; private set; } = "ciao!";

		#endregion

		public void On() => State = LightState.On;
		public void Off() => State = LightState.Off;
		public string Get() => State == LightState.On ? "ON" : "OFF";
		public void Color(string hexColor) => HexColor = hexColor;
		public void End() => Entity.Current.DestructOnExit();

		[FunctionName(nameof(Light))]
		public static Task Run(
			[EntityTrigger] IDurableEntityContext ctx,
			ILogger log)
			=> ctx.DispatchAsync<Light>(log);
	}

	public class Trigger
	{
		[FunctionName("LightTrigger")]
		public static async Task<IActionResult> LightTrigger(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "lights/{lightId}")] HttpRequest req,
			string lightId,
			[DurableClient] IDurableEntityClient client,
			ILogger log)
		{
			log.LogInformation("C# HTTP trigger function processed a request.");

			var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			var lightRequest = JsonConvert.DeserializeObject<LightRequest>(requestBody);

			var entityId = new EntityId(nameof(Light), lightId);

			await client.SignalEntityAsync(entityId, lightRequest.LightAction.ToString(), 
				lightRequest.LightAction == LightAction.Color ? lightRequest.HexColor : null);

			return new AcceptedResult();
		}
	}
}
