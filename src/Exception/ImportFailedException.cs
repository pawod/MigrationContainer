using Pawod.MigrationContainer.Container.Header.Base;

namespace Pawod.MigrationContainer.Exception
{
    public class ImportFailedException : System.Exception
    {
        public readonly string ContentName;

        public ImportFailedException(IFileHeader fileHeader, System.Exception ex)
            : base($"Failed to import content: '{fileHeader.OriginalName}'", ex)
        {
            ContentName = fileHeader.OriginalName;
        }
    }
}