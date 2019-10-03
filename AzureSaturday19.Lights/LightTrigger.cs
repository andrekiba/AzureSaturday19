﻿using AzureSaturday19.Lights.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace AzureSaturday19.Lights
{
	public class LightTrigger
	{
		[FunctionName("LightTrigger")]
		public static async Task<IActionResult> Run(
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