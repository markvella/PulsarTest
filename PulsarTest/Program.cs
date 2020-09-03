using System;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Microsoft.FSharp.Collections;
using Pulsar.Client.Api;
using Pulsar.Client.Common;

namespace PulsarTest
{
    public class Test
    {
        public string Text { get; set; }
    }
    class Program
    {
        private static PulsarClient client;
        private static string topicName;
        private static bool running=true;

        static async Task Read()
        {
            
            var consumer = await client.NewConsumer(Schema.JSON<Test>())
                .Topic(topicName)
                .SubscriptionName("test")
                .SubscribeAsync();

            while (!consumer.HasReachedEndOfTopic && running)
            {
                var message = await consumer.ReceiveAsync();
                Console.WriteLine($"Received: {message.GetValue().Text}");

                await consumer.AcknowledgeAsync(message.MessageId);
            }
        }

        static async Task WaitKey()
        {
            //Console.ReadKey();
            //running = false;
        }

        static async Task Publish()
        {
            
            var producer = await client.NewProducer(Schema.JSON<Test>())
                .Topic(topicName)
                .CreateAsync();
            
            while (running)
            {
                await producer.SendAsync(new Test {Text = DateTime.UtcNow.ToString()});
                await Task.Delay(200);
            }

            //Console.WriteLine($"MessageId is: '{messageId}'");
        }
        static async Task Main(string[] args)
        {
            client = new Pulsar.Client.Api.PulsarClientBuilder()
                .ServiceUrl("pulsar://pulsar:6650")
                .Build();
            topicName = "stringTopic";
            await Task.WhenAny(Read(), Publish());
        }
    }
}
