using Microsoft.Extensions.Configuration;
using System.IO;

namespace AzureSaturday19.KEDA.Pusher
{
	public class AppSettings
	{
		public string StorageConnectionString { get; }

		readonly IConfigurationRoot config;

		public AppSettings()
		{ 
			config = new ConfigurationBuilder()
			   .SetBasePath(Directory.GetCurrentDirectory())
			   .AddJsonFile("appsettings.json", false, true)
			   .Build();

			StorageConnectionString = config.GetValue<string>("StorageConnectionString");
		}

		public string GetValue(string key) => config.GetValue<string>(key);
	}
}
