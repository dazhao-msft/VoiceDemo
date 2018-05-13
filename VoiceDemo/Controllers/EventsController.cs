using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        public void Post()
        {
            _logger.LogInformation("event posted.");
        }
    }
}