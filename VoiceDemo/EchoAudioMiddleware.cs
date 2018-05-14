using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net.WebSockets;
using System.Text;
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
                using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync())
                {
                    try
                    {
                        await EchoAsync(context, webSocket, logger);
                    }
                    catch (OperationCanceledException)
                    {
                        // TODO: log additional info.
                    }
                }
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
                var result = await webSocket.ReceiveAsync(buffer, context.RequestAborted);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, context.RequestAborted);
                    break;
                }
                else if (result.MessageType == WebSocketMessageType.Text)
                {
                    logger.LogInformation(Encoding.UTF8.GetString(buffer.Slice(0, result.Count).Span));
                }
                else if (result.MessageType == WebSocketMessageType.Binary)
                {
                    await webSocket.SendAsync(buffer.Slice(0, result.Count), WebSocketMessageType.Binary, result.EndOfMessage, context.RequestAborted);
                }
                else
                {
                    throw new IndexOutOfRangeException($"Unrecognized WebSocketMessageType: {result.MessageType}.");
                }
            }
        }
    }
}
