using HighFrequencyDataFlowReceiver.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HighFrequencyDataFlowReceiver.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataReceiverController : ControllerBase
    {
        private readonly ILogger<DataReceiverController> _logger;

        public DataReceiverController(ILogger<DataReceiverController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "HealthCheck")]
        public IActionResult HealthCheck()
        {
            return Ok("We're alive and healthy :)");
        }

        [HttpPost(Name = "Grab")]
        public IActionResult Grab([FromBody] SomeDataObject someDataObject)
        {
            Console.WriteLine($"{JsonConvert.SerializeObject(someDataObject)}");
            return Ok();
        }
    }
}