namespace Pawod.MigrationContainer.Exception
{
    public class ContainerTooSmallException : System.Exception
    {
        public readonly long MainPartHeadersLength;
        public readonly long MaxFileLength;

        public ContainerTooSmallException(long maxfileLength, long mainPartHeadersLength) : base(CreateMessage(maxfileLength, mainPartHeadersLength))
        {
            MaxFileLength = maxfileLength;
            MainPartHeadersLength = mainPartHeadersLength;
        }

        private static string CreateMessage(long maxfileLength, long mainPartHeadersLength)
        {
            return
                $"The specified maximum file length is too small to fit all headers in the main file. MaxFileLength: {maxfileLength} bytes. Required: {mainPartHeadersLength} bytes.";
        }
    }
}