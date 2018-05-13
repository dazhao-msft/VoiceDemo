using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VoiceDemo
{
    public class EchoAudioMiddleware
    {
        private readonly RequestDelegate _next;

        public EchoAudioMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILogger<EchoAudioMiddleware> logger)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

                await EchoAsync(context, webSocket, logger);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private async Task EchoAsync(HttpContext context, WebSocket webSocket, ILogger logger)
        {
            Memory<byte> buffer = new byte[4 * 1024];

            while (true)
            {
                var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    break;
                }
                else if (result.MessageType == WebSocketMessageType.Text)
                {
                    logger.LogInformation(Encoding.UTF8.GetString(buffer.Slice(0, result.Count).Span));
                }
                else if (result.MessageType == WebSocketMessageType.Binary)
                {
                    await webSocket.SendAsync(buffer, WebSocketMessageType.Binary, false, CancellationToken.None);
                }
                else
                {
                    throw new IndexOutOfRangeException($"Unrecognized WebSocketMessageType: {result.MessageType}.");
                }
            }
        }
    }
}
