using System;
using System.Threading.Tasks;
using AzureSaturday19.Lights.Base;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AzureSaturday19.Lights
{
	public static class LightUntyped
	{
		[FunctionName("LightUntyped")]
		public static Task LightUntypedRun(
			[EntityTrigger] IDurableEntityContext ctx,
			ILogger log)
		{
			try
			{
				if (ctx.IsNewlyConstructed)
					ctx.SetState(new { State = LightState.Off, HexColor = "#eeeeee"});
				
				var currentState = ctx.GetState<dynamic>();

				switch (ctx.OperationName)
				{
					case "On":
						currentState.State = LightState.On;
						break;
					case "Off":
						currentState.State = LightState.Off;
						break;
					case "Color":
						var hexColor = ctx.GetInput<string>();
						currentState.HexColor = hexColor;
						break;
					case "Get":
						ctx.Return(currentState);
						break;
					case "End":
						ctx.DestructOnExit();
						break;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			return Task.CompletedTask;
		}
	}
}
