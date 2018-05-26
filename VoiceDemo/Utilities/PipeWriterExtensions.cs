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
                return pipeWriter.WriteMultiSegmentsAsync(source, cancellationToken);
            }
        }

        private static async ValueTask<FlushResult> WriteMultiSegmentsAsync(this PipeWriter pipeWriter, ReadOnlySequence<byte> source, CancellationToken cancellationToken = default)
        {
            FlushResult flushResult = default;

            SequencePosition position = source.Start;

            while (source.TryGet(ref position, out var segment))
            {
                flushResult = await pipeWriter.WriteAsync(segment, cancellationToken);

                if (flushResult.IsCanceled)
                {
                    return flushResult;
                }
            }

            return flushResult;
        }
    }
}
