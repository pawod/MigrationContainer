namespace DataMigrator.Exception
{
    using System;

    public class InvalidContainerException : Exception
    {
        public InvalidContainerException(string reason = null)
            : base(
                string.Format("The specified file is either not a valid MigrationContainer or has been corrupted. {0}",
                    string.Format("{0}: {1}", string.IsNullOrWhiteSpace(reason)? string.Empty : "Reason: ", reason)))
        {
        }
    }
}
