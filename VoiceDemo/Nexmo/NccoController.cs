using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace VoiceDemo.Nexmo
{
    [Route("api/[controller]")]
    public class NccoController : Controller
    {
        private const string PayloadTemplate =
@"[
    {
        ""action"": ""talk"",
        ""text"": ""Please wait while we connect you""
    },
    {
        ""action"": ""connect"",
        ""eventUrl"": [
            ""https://{{HOST}}/api/events""
        ],
        ""from"": ""12014317290"",
        ""endpoint"": [
            {
                ""type"": ""websocket"",
                ""uri"": ""wss://{{HOST}}/ws/echo"",
                ""content-type"": ""audio/l16;rate=16000"",
                ""headers"": {
                    ""session_id"": ""{{SESSION_ID}}""
                }
            }
        ]
    }
]";

        [HttpGet]
        public IActionResult Get()
        {
            string payload = PayloadTemplate.Replace("{{HOST}}", Request.Host.Value)
                                            .Replace("{{SESSION_ID}}", Guid.NewGuid().ToString());

            return Json(JsonConvert.DeserializeObject(payload));
        }
    }
}