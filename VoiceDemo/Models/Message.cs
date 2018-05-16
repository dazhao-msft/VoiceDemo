using System;
using System.Collections.Immutable;
using System.Text;

namespace VoiceDemo.Models
{
    public abstract class Message
    {
        public ImmutableDictionary<string, string> Headers { get; set; } = ImmutableDictionary<string, string>.Empty;

        public string Path
        {
            get
            {
                return Headers["Path"];
            }
            set
            {
                Headers = Headers.SetItem("Path", value);
            }
        }

        public Guid RequestId
        {
            get
            {
                return Guid.Parse(Headers["X-RequestId"]);
            }
            set
            {
                Headers = Headers.SetItem("X-RequestId", value.ToString("N"));
            }
        }
    }

    public abstract class Message<T> : Message
    {
        public T Body { get; set; }
    }

    public sealed class TextMessage : Message<string>
    {
    }

    public sealed class BinaryMessage : Message<byte[]>
    {
    }

    public abstract class MessageSerializer<TMessage> where TMessage : Message
    {
        protected readonly string Delimiter = "\r\n";
        protected readonly string KeyValueDelimiter = ":";

        protected readonly Encoding _encoding;

        protected MessageSerializer(Encoding encoding) => _encoding = encoding;

        public abstract TMessage Deserialize(ReadOnlySpan<byte> buffer);

        public abstract ReadOnlySpan<byte> Serialize(TMessage message, Span<byte> buffer);
    }

    public sealed class TextMessageSerializer : MessageSerializer<TextMessage>
    {
        public TextMessageSerializer()
            : base(Encoding.UTF8)
        {
        }

        public override TextMessage Deserialize(ReadOnlySpan<byte> buffer)
        {
            var message = new TextMessage();

            string[] values = _encoding.GetString(buffer).Split(Delimiter + Delimiter, StringSplitOptions.RemoveEmptyEntries);

            foreach (string header in values[0].Split(Delimiter, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] keyValue = header.Split(KeyValueDelimiter, StringSplitOptions.RemoveEmptyEntries);

                message.Headers = message.Headers.Add(keyValue[0], keyValue[1]);
            }

            message.Body = values[1];

            return message;
        }

        public override ReadOnlySpan<byte> Serialize(TextMessage message, Span<byte> buffer)
        {
            int index = 0;

            foreach (var header in message.Headers)
            {
                index += _encoding.GetBytes(header.Key, buffer.Slice(index));
                index += _encoding.GetBytes(KeyValueDelimiter, buffer.Slice(index));
                index += _encoding.GetBytes(header.Value, buffer.Slice(index));
                index += _encoding.GetBytes(Delimiter, buffer.Slice(index));
            }

            index += _encoding.GetBytes(Delimiter, buffer.Slice(index));

            index += _encoding.GetBytes(message.Body, buffer.Slice(index));

            return buffer.Slice(0, index);
        }
    }

    public sealed class BinaryMessageSerializer : MessageSerializer<BinaryMessage>
    {
        public BinaryMessageSerializer()
            : base(Encoding.UTF8)
        {
        }

        public override BinaryMessage Deserialize(ReadOnlySpan<byte> buffer)
        {
            var message = new BinaryMessage();

            int headersLength = buffer[0] << 8 | buffer[1];

            string headers = _encoding.GetString(buffer.Slice(2, headersLength));

            foreach (string header in headers.Split(Delimiter, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] keyValue = header.Split(KeyValueDelimiter, StringSplitOptions.RemoveEmptyEntries);

                message.Headers = message.Headers.Add(keyValue[0], keyValue[1]);
            }

            message.Body = buffer.Slice(2 + headersLength).ToArray();

            return message;
        }

        public override ReadOnlySpan<byte> Serialize(BinaryMessage message, Span<byte> buffer)
        {
            int index = 2;

            foreach (var header in message.Headers)
            {
                index += _encoding.GetBytes(header.Key, buffer.Slice(index));
                index += _encoding.GetBytes(KeyValueDelimiter, buffer.Slice(index));
                index += _encoding.GetBytes(header.Value, buffer.Slice(index));
                index += _encoding.GetBytes(Delimiter, buffer.Slice(index));
            }

            int headersLength = index - 2;

            buffer[0] = (byte)((headersLength & 65280) >> 8);
            buffer[1] = (byte)(headersLength & 255);

            message.Body.AsSpan().CopyTo(buffer.Slice(index));

            return buffer.Slice(0, index + message.Body.Length);
        }
    }
}
