namespace DataMigrator.Helper
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CodeFluent.Runtime.BinaryServices;
    using Configuration;
    using Stream;

    public static class StreamExtensions
    {
        public static void CopyTo(this Stream source, Stream target, long count, int bufferSize)
        {
            var buffer = new byte[bufferSize];

            var bytesWritten = 0;
            while (bytesWritten + bufferSize <= count)
            {
                source.Read(buffer, 0, bufferSize);
                target.Write(buffer, 0, bufferSize);
                bytesWritten += bufferSize;
            }
            if (bytesWritten >= count) return;

            var remainingBytes = (int)(count - bytesWritten);
            source.Read(buffer, 0, remainingBytes);
            target.Write(buffer, 0, remainingBytes);
        }

        public static void CopyTo(this Stream source,
                                  Stream target,
                                  long sourceOffset,
                                  long targetOffset,
                                  long count,
                                  int bufferSize)
        {
            source.Position = sourceOffset;
            target.Position = targetOffset;
            source.CopyTo(target, count, bufferSize);
        }

        public static void DisposeAll(this IEnumerable<Stream> streams)
        {
            foreach (var stream in streams)
            {
                stream.Dispose();
            }
        }

        public static IEnumerable<NtfsAlternateStream> GetAlternateStreams(this FileSystemInfo fsInfo)
        {
            return
                NtfsAlternateStream.EnumerateStreams(fsInfo.FullName)
                                   .Where(
                                       s =>
                                           s.StreamType.HasFlag(NtfsAlternateStreamType.AlternateData) && s.Name != null);
        }

        public static SubStream GetSubStream(this Stream stream, long offset, long length)
        {
            return new SubStream(stream, offset, length);
        }

        public static bool ReachedEnd(this Stream stream)
        {
            return stream.Position == stream.Length;
        }

        public static bool StreamEquals(this Stream stream1, Stream stream2)
        {
            const int bufferSize = 4096;
            var buffer1 = new byte[bufferSize];
            var buffer2 = new byte[bufferSize];
            while (true)
            {
                var count1 = stream1.Read(buffer1, 0, bufferSize);
                var count2 = stream2.Read(buffer2, 0, bufferSize);

                if (count1 != count2) return false;
                if (count1 == 0) return true;
                if (!buffer1.Take(count1).SequenceEqual(buffer2.Take(count2))) return false;
            }
        }
    }
}
