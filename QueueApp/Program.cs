using System;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace QueueApp
{
    class Program
    {
        private static string connectionString = "DefaultEndpointsProtocol=https;EndpointSuffix=core.windows.net;AccountName=articlesjari001;AccountKey=YYhvGj96uAcLiwHJMK7br4ww+x5GPsfLseimWA5yxTUjlTezXs2eW5z+nkM9SfgFU/aZ1ZU7ohPwYtbwfSicAw==";
        private static string queueName = "articlesjari001";

        private static QueueClient client;

        static async Task Main(string[] args)
        {
            Console.WriteLine("-- Starting...");

            client = new QueueClient(connectionString, queueName);

            Response response = await client.CreateIfNotExistsAsync();

            //await SendArticleAsync("Testing 1");
            //await SendArticleAsync("Testing 2");

            await ReceiveArticleAsync();


            Console.WriteLine("-- Done (press enter)");
            Console.ReadLine();
        }

        static async Task SendArticleAsync(string message)
        {
            await client.SendMessageAsync(message);

            Console.WriteLine("-- send " + message);
        }

        static async Task ReceiveArticleAsync()
        {
            var resp = await client.ReceiveMessagesAsync(maxMessages: 3);

            foreach (var msg in resp.Value)
            {
                Console.WriteLine("-- receive " + msg.MessageText);

                await client.DeleteMessageAsync(msg.MessageId, msg.PopReceipt);
            }
        }
    }
}
