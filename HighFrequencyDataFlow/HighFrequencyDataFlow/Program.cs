// See https://aka.ms/new-console-template for more information
using HighFrequencyDataFlow;
using System.Diagnostics;

var stopWatch = new Stopwatch();
stopWatch.Start();

Console.WriteLine("Starting message send...");

var senderThread = new SenderThread("https://localhost:7201/DataReceiver");
var cancellationToken = new CancellationToken();
var taskList = new List<Task>();

var threadNumber = 0;
for (int i = 1; i <= 16; i++)
{
    var currentThreadNumber = threadNumber + i;
    taskList.Add(Task.Run(() => senderThread.RunSenderThread(cancellationToken, currentThreadNumber, 1000)));
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