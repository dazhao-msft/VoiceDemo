using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.SignalR.Internal;
using Microsoft.Extensions.Logging;
using System.Buffers;
using System.Text;
using System.Threading.Tasks;

namespace VoiceDemo
{
    public class EchoAudioHandler : ConnectionHandler
    {
        private readonly ILogger<EchoAudioHandler> _logger;

        public EchoAudioHandler(ILogger<EchoAudioHandler> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            string metadata = await ReadConnectionMetadataAsync(connection);

            LogConnectionMetadata(metadata);

            await EchoAudioAsync(connection);
        }

        private async Task<string> ReadConnectionMetadataAsync(ConnectionContext connection)
        {
            var result = await connection.Transport.Input.ReadAsync();

            Utf8BufferTextReader reader = null;
            try
            {
                reader = Utf8BufferTextReader.Get(result.Buffer);

                return await reader.ReadToEndAsync();
            }
            finally
            {
                Utf8BufferTextReader.Return(reader);

                connection.Transport.Input.AdvanceTo(result.Buffer.End);
            }
        }

        private void LogConnectionMetadata(string metadata)
        {
            var builder = new StringBuilder();

            builder.AppendLine("-------------------------------------------------------------");

            builder.AppendLine("Connection metadata:");

            builder.AppendLine(metadata);

            builder.AppendLine("-------------------------------------------------------------");

            _logger.LogInformation(builder.ToString());
        }

        private async Task EchoAudioAsync(ConnectionContext connection)
        {
            var transferFormatFeature = connection.Features.Get<ITransferFormatFeature>();
            transferFormatFeature.ActiveFormat = TransferFormat.Binary;

            while (true)
            {
                var result = await connection.Transport.Input.ReadAsync();
                var buffer = result.Buffer;

                try
                {
                    if (!buffer.IsEmpty)
                    {
                        await connection.Transport.Output.WriteAsync(buffer.ToArray());
                    }
                    else if (result.IsCompleted)
                    {
                        break;
                    }
                }
                finally
                {
                    connection.Transport.Input.AdvanceTo(result.Buffer.End);
                }
            }
        }
    }
}
