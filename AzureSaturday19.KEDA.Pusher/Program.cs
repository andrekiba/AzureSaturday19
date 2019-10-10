using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using System;
using System.Threading.Tasks;

namespace AzureSaturday19.KEDA.Pusher
{
	internal class Program
    {
        static async Task Main()
        {
            try
            {
				var appSettings = new AppSettings();

                var storageAccount = CloudStorageAccount.Parse(appSettings.StorageConnectionString);
                var queueClient = storageAccount.CreateCloudQueueClient();

                // Retrieve a reference to a container.
                var queue = queueClient.GetQueueReference("kedaqueue");

                // Create the queue if it doesn't already exist
                queue.CreateIfNotExists();

                for (var i = 0; i < 1000; i++)
                {
                    await queue.AddMessageAsync(new CloudQueueMessage("Ciao KEDA!"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
