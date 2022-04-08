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
        private readonly string _url = "https://localhost:7201/DataReceiver";
        public int Success { get; set; } = 0;
        public int Failure { get; set; } = 0;

        public async Task RunSenderThread(CancellationToken token, int threadNumber)
        {
            var httpClient = new HttpClient();
            var complete = false;
            var control = 1000;
            var dayCareData = await LoadDayCareData(control, threadNumber);
            var random = new Random();

            // Safe thread management
            while (!token.IsCancellationRequested && !complete)
            {
                Console.WriteLine($"Thread {threadNumber}: Running...");

                for (int i = 0; i <= control; i++)
                {
                    var payload = new KittyDayCare(dayCareData.CatNames[i], dayCareData.CatActivities[random.Next(0, (dayCareData.CatActivities.Count - 1))], DateTime.Now);
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

        private async Task<DayCareDataLoad> LoadDayCareData(int control, int threadNumber)
        {
            var baseFilePath = @$"{Directory.GetCurrentDirectory()}\Data";
            var catNames = (await File.ReadAllLinesAsync(@$"{baseFilePath}\CatNames.txt")).ToList();
            var dayCareActivities = (await File.ReadAllLinesAsync(@$"{Directory.GetCurrentDirectory()}\Data\Daycare_Activities.txt")).ToList();

            var startIndex = (threadNumber - 1) * control;

            if ((startIndex >= catNames.Count) || ((startIndex + control) >= catNames.Count))
            {
                startIndex = 0;
            }

            var currentCatNames = catNames.GetRange(startIndex, control);
            var data = new DayCareDataLoad()
            {
                CatNames = currentCatNames,
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
