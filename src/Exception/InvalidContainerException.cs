namespace Pawod.MigrationContainer.Exception
{
    public class InvalidContainerException : System.Exception
    {
        public InvalidContainerException(string reason = null)
            : base(
                $"The specified file is either not a valid MigrationContainer or has been corrupted. {(string.IsNullOrWhiteSpace(reason) ? string.Empty : "Reason: ")}: {reason}"
                )
        {
        }
    }
}