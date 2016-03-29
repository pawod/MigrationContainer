namespace DataMigrator.Exception
{
    using System;

    public class ContainerTooSmallException : Exception
    {
        public readonly long MainPartHeadersLength;
        public readonly long MaxFileLength;

        public ContainerTooSmallException(long maxfileLength, long mainPartHeadersLength)
            : base(CreateMessage(maxfileLength, mainPartHeadersLength))
        {
            MaxFileLength = maxfileLength;
            MainPartHeadersLength = mainPartHeadersLength;
        }

        private static string CreateMessage(long maxfileLength, long mainPartHeadersLength)
        {
            return
                string.Format(
                    "The specified maximum file length is too small to fit all headers in the main file. MaxFileLength: {0} bytes. Required: {1} bytes.",
                    maxfileLength,
                    mainPartHeadersLength);
        }
    }
}
