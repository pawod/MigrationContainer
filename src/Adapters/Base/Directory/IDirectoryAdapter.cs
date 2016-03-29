namespace DataMigrator.Adapters.Base.Directory
{
    using System.IO;
    using Container.Base.Body;
    using Container.Base.Header;
    using Container.DirectoryContainer.Base;
    using File;

    /// <summary>
    ///     Provides basic functionality for the import of whole directory structures.
    /// </summary>
    /// <typeparam name="TFileHeader">
    ///     The type of FileHeader associated with the files
    ///     contained inside the directory.
    /// </typeparam>
    /// <typeparam name="TDirectoryHeader">
    ///     The type of DirectoryHeader associated with
    ///     the directory to be imported.
    /// </typeparam>
    public interface IDirectoryAdapter<in TDirectoryHeader, in TFileHeader> : IFileAdapter<TFileHeader>
        where TDirectoryHeader : class, IDirectoryHeaderBase<TDirectoryHeader, TFileHeader>
        where TFileHeader : class, IFilesystemHeader<FileInfo>
    {
	    /// <summary>
	    ///     Imports the directory associated with the specified DirectoryHeader.
	    /// </summary>
	    /// <param name="body">The MigrationContainer's body.</param>
	    /// <param name="directoryHeader">The header of the directory to be imported.</param>
	    /// <param name="locationKey">
	    ///     The key that uniquely identifies the target location for
	    ///     the content to be imported.
	    /// </param>
	    /// <param name="customDirName">Allows to use a different name for the directory to be imported.</param>
	    /// <returns>The key, that uniquely identifies the imported directory.</returns>
	    string ImportDirectory(IContainerBody body, TDirectoryHeader directoryHeader, string locationKey, string customDirName = null);
    }
}
