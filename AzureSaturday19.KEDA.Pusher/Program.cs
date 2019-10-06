using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using System.Threading.Tasks;

namespace AzureSaturday19.KEDA.Pusher
{
    class Program
    {
        static async Task Main()
        {
            var storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
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
    }
}
