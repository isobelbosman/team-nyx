using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rabbit
{
    public class SenderThread
    {
        private readonly IModel _channel;
        public int Success { get; set; } = 0;
        public int Failure { get; set; } = 0;

        public SenderThread(IModel channel)
        {
            _channel = channel;
        }

        public async Task RunSenderThread(CancellationToken token, int threadNumber, int control)
        {
            var complete = false;
            var dayCareData = await LoadDayCareData();
            var random = new Random();

            // Safe thread management
            while (!token.IsCancellationRequested && !complete)
            {
                Console.WriteLine($"Thread {threadNumber}: Running...");

                var startIndex = control * (threadNumber - 1);
                for (int i = 0; i < control; i++)
                {
                    if (startIndex > (dayCareData.CatNames.Count - 1))
                    {
                        startIndex = 0;
                    }

                    try 
                    {
                        var payload = new KittyDayCare(dayCareData.CatNames[startIndex], dayCareData.CatActivities[random.Next(0, (dayCareData.CatActivities.Count - 1))], DateTime.Now);
                        var byteContent = PreparePayload(payload, threadNumber);

                        _channel.BasicPublish("", "demo-queue", null, byteContent);

                        Console.WriteLine($"Thread {threadNumber}: Post Successful...");
                        Success++;
                    }
                    catch (Exception ex) 
                    {
                        Console.WriteLine($"Thread {threadNumber}: Post Failure, Response: {ex.Message}...");
                        Failure++;
                    }

                    startIndex++;
                }
                complete = true;
            }
        }

        private byte[] PreparePayload(KittyDayCare payload, int threadNumber)
        {
            var myContent = JsonConvert.SerializeObject(payload);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);

            Console.WriteLine($"Thread {threadNumber}: Payload prepared - {JsonConvert.SerializeObject(payload)}");

            return buffer;
        }

        private async Task<DayCareDataLoad> LoadDayCareData()
        {
            var baseFilePath = @$"{Directory.GetCurrentDirectory()}\Data";
            var catNames = (await File.ReadAllLinesAsync(@$"{baseFilePath}\CatNames.txt")).ToList();
            var dayCareActivities = (await File.ReadAllLinesAsync(@$"{Directory.GetCurrentDirectory()}\Data\Daycare_Activities.txt")).ToList();

            var data = new DayCareDataLoad()
            {
                CatNames = catNames,
                CatActivities = dayCareActivities
            };

            return data;
        }
    }

    public class DayCareDataLoad
    {
        public List<string> CatNames { get; set; }
        public List<string> CatActivities { get; set; }
    }
}
