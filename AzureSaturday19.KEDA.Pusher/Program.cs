using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureSaturday19.KEDA.Pusher
{
    class Program
    {
        static async Task Main()
        {
            try
            {
                var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .Build();

                var connectionString = config.GetConnectionString("StorageConnectionString");

                var storageAccount = CloudStorageAccount.Parse(connectionString);
                var queueClient = storageAccount.CreateCloudQueueClient();

                // Retrieve a reference to a container.
                var queue = queueClient.GetQueueReference("kedaqueue");

                // Create the queue if it doesn't already exist
                queue.CreateIfNotExists();

                for (int i = 0; i < 1000; i++)
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
