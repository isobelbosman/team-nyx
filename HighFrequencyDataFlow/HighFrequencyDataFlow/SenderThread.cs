using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HighFrequencyDataFlow
{
    public class SenderThread
    {
        private readonly string _url;
        public int Success { get; set; } = 0;
        public int Failure { get; set; } = 0;

        public SenderThread(string url)
        {
            _url = url;
        }

        public async Task RunSenderThread(CancellationToken token, int threadNumber, int control)
        {
            var httpClient = new HttpClient();
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

                    var payload = new KittyDayCare(dayCareData.CatNames[startIndex], dayCareData.CatActivities[random.Next(0, (dayCareData.CatActivities.Count - 1))], DateTime.Now);
                    var byteContent = PreparePayload(payload, threadNumber);

                    var response = await httpClient.PostAsync(_url, byteContent);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Thread {threadNumber}: Post Successful...");
                        Success++;
                    }
                    else
                    {
                        Console.WriteLine($"Thread {threadNumber}: Post Failure, Response: {response.StatusCode} - {response.ReasonPhrase}...");
                        Failure++;
                    }

                    startIndex++;
                }
                complete = true;
            }

            httpClient.Dispose();
        }

        private ByteArrayContent PreparePayload(KittyDayCare payload, int threadNumber)
        {
            var myContent = JsonConvert.SerializeObject(payload);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            Console.WriteLine($"Thread {threadNumber}: Payload prepared - {JsonConvert.SerializeObject(payload)}");

            return byteContent;
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
