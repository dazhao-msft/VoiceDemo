using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace VoiceDemo.Utilities
{
    internal static class PipeWriterExtensions
    {
        public static ValueTask<FlushResult> WriteAsync(this PipeWriter pipeWriter, ReadOnlySequence<byte> source, CancellationToken cancellationToken = default)
        {
            if (source.IsSingleSegment)
            {
                return pipeWriter.WriteAsync(source.First, cancellationToken);
            }
            else
            {
                return SendMultiSegmentAsync(pipeWriter, source, cancellationToken);
            }
        }

        private static async ValueTask<FlushResult> SendMultiSegmentAsync(PipeWriter pipeWriter, ReadOnlySequence<byte> source, CancellationToken cancellationToken)
        {
            var position = source.Start;
            while (source.TryGet(ref position, out var segment))
            {
                await pipeWriter.WriteAsync(segment, cancellationToken);
            }

            // Empty end of message frame
            return await pipeWriter.WriteAsync(Memory<byte>.Empty, cancellationToken);
        }
    }
}
