using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Producer;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventHub
{
    class Program
    {
        private static string connectionString = "Endpoint=sb://ehubns-309.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=IbJ9JZBP75ZbkEsbmGndMC4iTGLMEEDqFA4repdsxtY=";
        private static string eventHubName = "hubname-19777";

        static async Task Main(string[] args)
        {
            Console.WriteLine("EventHub exercise");

            //await SendAll();
            await ReadEvents();
        }

        static async Task SendAll()
        {
            await using (var producer = new EventHubProducerClient(connectionString, eventHubName))
            {
                int counter = 1;
                foreach (string prefix in new string[] { "a", "b", "c", "d" }) {
                    await SendEvent(producer, $"{prefix}_Test_{counter}");
                    counter++;
                }
            }
        }

        static async Task SendEvent(EventHubProducerClient client, string message)
        {
            EventDataBatch batch = await client.CreateBatchAsync();

            batch.TryAdd(new EventData(Encoding.UTF8.GetBytes(message)));

            await client.SendAsync(batch);
        }

        static async Task ReadEvents()
        {
            string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

            await using (var consumer = new EventHubConsumerClient(consumerGroup, connectionString, eventHubName))
            {
                using var cancellationSource = new CancellationTokenSource();
                cancellationSource.CancelAfter(TimeSpan.FromSeconds(45));

                await foreach (PartitionEvent ev in consumer.ReadEventsAsync(cancellationSource.Token))
                {
                    Console.WriteLine("EVENT = " + Encoding.UTF8.GetString(ev.Data.Body.ToArray()));
                }
            }
        }
    }
}

