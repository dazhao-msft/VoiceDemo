using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using VoiceDemo.Nexmo;

namespace VoiceDemo.Controllers
{
    [Route("api/[controller]")]
    public class EventsController : Controller
    {
        private readonly ILogger<EventsController> _logger;

        public EventsController(ILogger<EventsController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task PostAsync()
        {
            var builder = new StringBuilder();

            builder.AppendLine("-------------------------------------------------------------");

            builder.AppendLine("Request header:");

            foreach (var header in Request.Headers)
            {
                builder.AppendLine($"{header.Key} : {header.Value}");
            }

            builder.AppendLine();

            builder.AppendLine("Request body:");

            Memory<byte> buffer = new byte[4 * 1024];
            int bytesRead = await Request.Body.ReadAsync(buffer);
            string requestBody = Encoding.UTF8.GetString(buffer.Span.Slice(0, bytesRead));
            builder.AppendLine(requestBody);
            var nexmoEvent = JsonConvert.DeserializeObject<NexmoEvent>(requestBody);
            builder.AppendLine(JsonConvert.SerializeObject(nexmoEvent));

            builder.AppendLine("-------------------------------------------------------------");

            _logger.LogInformation(builder.ToString());
        }
    }
}