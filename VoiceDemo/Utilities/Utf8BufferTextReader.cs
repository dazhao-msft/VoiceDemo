using System;
using System.Buffers;
using System.IO;
using System.Text;

namespace VoiceDemo.Utilities
{
    internal sealed class Utf8BufferTextReader : TextReader
    {
        private readonly Decoder _decoder;
        private ReadOnlySequence<byte> _utf8Buffer;

        [ThreadStatic]
        private static Utf8BufferTextReader _cachedInstance;

        public Utf8BufferTextReader()
        {
            _decoder = Encoding.UTF8.GetDecoder();
        }

        public static Utf8BufferTextReader Get(in ReadOnlySequence<byte> utf8Buffer)
        {
            var reader = _cachedInstance;
            if (reader == null)
            {
                reader = new Utf8BufferTextReader();
            }

            // Taken off the the thread static
            _cachedInstance = null;
            reader.SetBuffer(utf8Buffer);
            return reader;
        }

        public static void Return(Utf8BufferTextReader reader)
        {
            _cachedInstance = reader;
        }

        public void SetBuffer(in ReadOnlySequence<byte> utf8Buffer)
        {
            _utf8Buffer = utf8Buffer;
            _decoder.Reset();
        }

        public override int Read(char[] buffer, int index, int count)
        {
            if (_utf8Buffer.IsEmpty)
            {
                return 0;
            }

            var source = _utf8Buffer.First.Span;
            var bytesUsed = 0;
            var charsUsed = 0;

            var destination = new Span<char>(buffer, index, count);
            _decoder.Convert(source, destination, false, out bytesUsed, out charsUsed, out var completed);

            _utf8Buffer = _utf8Buffer.Slice(bytesUsed);

            return charsUsed;
        }
    }
}
