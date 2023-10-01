using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;

namespace ChatBot.ToGPT.Utility
{
    public class ServiceBusUtility
    {
        private static readonly string queueName = "message";
        private static readonly ServiceBusClient clinet = new ServiceBusClient(Environment.GetEnvironmentVariable("ServiceBusConnectionString"));

        public static async Task SendServiceBusQueue(ILogger log, string message)
        {
            try
            {
                log.LogInformation($"Start Register ServiceBus Queue\nMessage:{message}");
                ServiceBusSender sender = clinet.CreateSender(queueName);

                // Create Batch
                using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

                // try add a message to the batch
                if (!messageBatch.TryAddMessage(new ServiceBusMessage(message)))
                {
                    log.LogError($"Exception {message} has occured");
                }
                await sender.SendMessagesAsync(messageBatch);
                log.LogInformation("End Register ServiceBus Queue");
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
            }
        }
    }
}