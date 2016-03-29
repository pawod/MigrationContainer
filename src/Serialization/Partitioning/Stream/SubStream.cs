namespace DataMigrator.Stream
{
    using System;
    using System.IO;

    /// <summary>
    ///     Provides access to a subsequence of a stream.
    ///     Credits to Marc Gravell:
    ///     https://social.msdn.microsoft.com/Forums/vstudio/en-US/c409b63b-37df-40ca-9322-458ffe06ea48/how-to-access-part-of-a-filestream-or-memorystream?forum=netfxbcl
    /// </summary>
    public class SubStream : Stream
    {
        private readonly long _length;
        private Stream _baseStream;
        private long _position;

        public override bool CanRead
        {
            get
            {
                CheckDisposed();
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                CheckDisposed();
                return true;
            }
        }

        public override bool CanWrite
        {
            get
            {
                CheckDisposed();
                return false;
            }
        }

        public override long Length
        {
            get
            {
                CheckDisposed();
                return _length;
            }
        }

        public override long Position
        {
            get
            {
                CheckDisposed();
                return _position;
            }
            set
            {
                CheckDisposed();
                if (value < 0) throw new ArgumentOutOfRangeException("value");
                _position = value;
            }
        }

        public SubStream(Stream baseStream, long offset, long length)
        {
            if (baseStream == null) throw new ArgumentNullException("baseStream");
            if (!baseStream.CanRead) throw new ArgumentException("can't read base stream");
            if (offset < 0) throw new ArgumentOutOfRangeException("offset");

            _baseStream = baseStream;
            _length = length;

            if (baseStream.CanSeek) baseStream.Seek(offset, SeekOrigin.Current);
            else SeekManually(baseStream, offset);
        }

        public override void Flush()
        {
            CheckDisposed();
            _baseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            CheckDisposed();
            var remaining = _length - _position;
            if (remaining <= 0) return 0;
            if (remaining < count) count = (int)remaining;
            var read = _baseStream.Read(buffer, offset, count);
            _position += read;
            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position -= offset;
                    break;
            }
            return Position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;
            if (_baseStream == null) return;
            _baseStream.Dispose();
            _baseStream = null;
        }

        private void CheckDisposed()
        {
            if (_baseStream == null) throw new ObjectDisposedException(GetType().Name);
        }

        private void SeekManually(Stream baseStream, long offset)
        {
            // read it manually...
            const int bufferSize = 512;
            var buffer = new byte[bufferSize];
            while (offset > 0)
            {
                var read = baseStream.Read(buffer, 0, offset < bufferSize? (int)offset : bufferSize);
                offset -= read;
            }
        }
    }
}
