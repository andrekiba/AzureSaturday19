using System.Threading.Tasks;
using AzureSaturday19.Lights.Base;
using Microsoft.Azure.WebJobs;
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
}
