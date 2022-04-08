using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

            var stopWatch = new Stopwatch();

            // Number of records to run
            var count = 1000;
            var runUsingThreads = false;

            var senderThread = new SenderThread(channel);
            var cancellationToken = new CancellationToken();
            var taskList = new List<Task>();

            Console.WriteLine("Starting message send...");
            stopWatch.Start();

            if (runUsingThreads)
            {
                var threadNumber = 0;
                for (int i = 1; i <= 16; i++)
                {
                    var currentThreadNumber = threadNumber + i;
                    taskList.Add(Task.Run(() => senderThread.RunSenderThread(cancellationToken, currentThreadNumber, count)));
                }
            }
            else
            {
                taskList.Add(Task.Run(() => senderThread.RunSender(cancellationToken, count)));
            }

            var allTasksCompleted = false;
            while (!allTasksCompleted)
            {
                allTasksCompleted = true;
                foreach (var task in taskList)
                {
                    if (!task.IsCompleted)
                    {
                        allTasksCompleted = false;
                    }
                }
            }

            stopWatch.Stop();

            var callsPerSecond = Math.Floor((senderThread.Success + senderThread.Failure) / Math.Ceiling((decimal)(stopWatch.ElapsedMilliseconds / 1000)));

            Console.WriteLine($"Successful Calls: {senderThread.Success} \nFailed Calls: {senderThread.Failure} \nCalls per second (floor): {callsPerSecond}");
            Console.WriteLine($"Message send complete in {stopWatch.ElapsedMilliseconds} milliseconds...");
        }
    }
}
