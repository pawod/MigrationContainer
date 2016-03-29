namespace DataMigrator.Exception
{
    using System;
    using Container.Base.Header;

    public class ImportFailedException : Exception
    {
        public readonly string ContentName;

        public ImportFailedException(IContentHeader contentHeader, Exception ex)
            : base(string.Format("Failed to import content: '{0}'", contentHeader.OriginalName), ex)
        {
            ContentName = contentHeader.OriginalName;
        }
    }
}
