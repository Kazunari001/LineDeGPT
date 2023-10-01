using System;
using System.Threading.Tasks;
using ChatBot.BackEnd.Model;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ChatBot.BackEnd
{
    public class RegisterCosmosDB
    {
        private static Lazy<CosmosClient> lazyClient = new Lazy<CosmosClient>(InitializeCosmosClient);
        private static CosmosClient cosmosClient => lazyClient.Value;

        private static CosmosClient InitializeCosmosClient()
        {
            // Perform any initialization here
            var uri = Environment.GetEnvironmentVariable("COSMOSENDPOINT");
            var authKey = Environment.GetEnvironmentVariable("COSMOSKEY");

            return new CosmosClient(uri, authKey);
        }

        [FunctionName("RegisterCosmosDB")]
        public async Task Run([ServiceBusTrigger("message", Connection = "ServiceBusConnectionString")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

            Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(id: Environment.GetEnvironmentVariable("COSMOSDATABASE"));

            Container container = await database.CreateContainerIfNotExistsAsync(
                id: "products",
                partitionKeyPath: "/categoryId"
            );

            log.LogInformation($"Success Create Container:{container.Id}");

            string id = Guid.NewGuid().ToString();
            string partitionKey = Guid.NewGuid().ToString();

            Product newItem = new (
                id: id,
                categoryId: partitionKey,
                categoryName: myQueueItem,
                name: "gpt"
            );

            Product createdItem = await container.CreateItemAsync(
                item: newItem,
                partitionKey: new PartitionKey(partitionKey)
            );

            log.LogInformation($"Created item:\t{createdItem.id}\t[{createdItem.categoryName}]");
        }
    }
}
