using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Rabbit
{
    static class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672")
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare("demo-queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            for (int i = 0; i < 10000000; i++)
            {
                var message = new { Name = "Producer", Message = $"Message number {i}, Time: {DateTime.Now.ToLongTimeString()}" };
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                channel.BasicPublish("", "demo-queue", null, body);
            }


        }
    }
}
