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
        public async Task RunSenderThread(CancellationToken token, int threadNumber)
        {
            var httpClient = new HttpClient();
            var complete = false;

            // Safe thread management
            while (!token.IsCancellationRequested && !complete)
            {
                Console.WriteLine($"Thread {threadNumber}: Running...");
                for (int i = 0; i <= 1000; i++)
                {
                    var payload = new SomeDataObject("a", "b", "c", "d");
                    var byteContent = PreparePayload(payload, threadNumber);

                    var response = await httpClient.PostAsync("https://localhost:7201/DataReceiver", byteContent);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Thread {threadNumber}: Post Successful...");
                    }
                    else
                    {
                        Console.WriteLine($"Thread {threadNumber}: Post Failure, Response: {response.StatusCode} - {response.ReasonPhrase}...");
                    }
                }
                complete = true;
            }

            httpClient.Dispose();
        }

        private ByteArrayContent PreparePayload(SomeDataObject payload, int threadNumber)
        {
            var myContent = JsonConvert.SerializeObject(payload);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            Console.WriteLine($"Thread {threadNumber}: Payload prepared - {JsonConvert.SerializeObject(payload)}");

            return byteContent;
        }
    }
}
