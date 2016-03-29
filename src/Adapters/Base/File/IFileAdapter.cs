namespace DataMigrator.Adapters.Base.File
{
    using System.IO;
    using Container.Base.Body;
    using Container.Base.Header;

    /// <summary>
    ///     Provides basic functionality for the import of files.
    /// </summary>
    /// <typeparam name="TFileHeader">
    ///     The thype of FileHeader associated with the file to
    ///     be imported
    /// </typeparam>
    public interface IFileAdapter<in TFileHeader>
        where TFileHeader : IFilesystemHeader<FileInfo>
    {
        /// <summary>
        ///     Imports a specific file from the MigrationContainer's content to the target
        ///     environment.
        /// </summary>
        /// <param name="fileHeader">The FileHeader, which describes the contained file.</param>
        /// <param name="body">The MigrationContainer's body.</param>
        /// <param name="locationKey">
        ///     The key that uniquely identifies the target location for
        ///     the file to be imported.
        /// </param>
        /// <returns>The key, that uniquely identifies the imported file.</returns>
        string ImportFile(TFileHeader fileHeader, IContainerBody body, string locationKey);
    }
}
