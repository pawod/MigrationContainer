namespace DataMigrator.Stream
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    ///     Allows to read a collection of SubStreams as if it was a single stream.
    /// </summary>
    public class CompositeStream : Stream
    {
        private readonly long _length;
        private int _currentIndex;
        private long _position;
        private List<SubStream> _subStreams;

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

        private long CurrentRemaining
        {
            get { return CurrentSubStream.Length - CurrentSubStream.Position; }
        }

        private Stream CurrentSubStream
        {
            get { return _subStreams[_currentIndex]; }
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
                if (_position > 0) PrepareSubStream(_position);
            }
        }

        private long TotalRemaining
        {
            get { return _length - Position; }
        }

        public CompositeStream(List<SubStream> subStreams)
        {
            if (subStreams == null) throw new ArgumentNullException("subStreams");

            _subStreams = subStreams;
            _length = _subStreams.Sum(s => s.Length);
            _currentIndex = 0;
        }

        public override void Flush()
        {
            foreach (var subStream in _subStreams)
            {
                subStream.Flush();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            CheckDisposed();
            if (TotalRemaining <= 0) return 0;
            if (CurrentRemaining <= 0 && _currentIndex < _subStreams.Count - 1) _currentIndex++;

            var read = ReadCurrentSubStream(buffer, offset, count);

			// substream ended, continue reading from the next one, to fill the buffer
            while (read < count && _currentIndex < _subStreams.Count - 1)
            {
                _currentIndex++;
                var remainingCount = count - read;
				read += CurrentSubStream.Read(buffer, offset+read, remainingCount);
            }
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
            if (_subStreams == null) return;

            foreach (var subStream in _subStreams)
            {
                subStream.Dispose();
            }
            _subStreams = null;
        }

        private void CheckDisposed()
        {
            if (_subStreams == null) throw new ObjectDisposedException(GetType().Name);
        }

        private void PrepareSubStream(long compositePosition)
        {
            var count = 0L;
            var index = 0;
            foreach (var subStream in _subStreams)
            {
                if (compositePosition <= count) break;
                count += subStream.Length;
                index++;
            }
            _currentIndex = index;
            var preceedingSubstreamLength = count - CurrentSubStream.Length;
            CurrentSubStream.Position = compositePosition - preceedingSubstreamLength;
        }

        private int ReadCurrentSubStream(byte[] buffer, int offset, int count)
        {
            if (CurrentRemaining < count) count = (int)CurrentRemaining;
            return CurrentSubStream.Read(buffer, offset, count);
        }
    }
}
