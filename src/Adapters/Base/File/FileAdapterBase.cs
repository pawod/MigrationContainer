namespace DataMigrator.Adapters.Base.File
{
    using System;
    using System.IO;
    using Container.Base.Body;
    using Container.Base.Header;
    using Container.FileContainer;
    using Exception;
    using NLog;

    public abstract class FileAdapterBase<TContainer, TFileHeader> : IContentAdapter<TContainer, TFileHeader, FileInfo>,
                                                                     IFileAdapter<TFileHeader>
        where TContainer : FileContainerInfoBase<TContainer, TFileHeader>
        where TFileHeader : class, INtfsFilesystemHeader<FileInfo>
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public string Import(TContainer container, string locationKey)
        {
            if (container.ContentHeader == null) throw new InvalidContainerException("ContentHeader is null.");
            return ImportFile(container.ContentHeader, container.Body, locationKey);
        }

        public string ImportFile(TFileHeader fileHeader, IContainerBody body, string locationKey)
        {
            try
            {
                var importKey = GetImportKey(fileHeader, locationKey);
                Logger.Trace("Importing file with name: '{0}' to location: '{1}'", fileHeader.OriginalName, locationKey);
                RestoreFile(fileHeader, body, importKey);
                return importKey;
            }
            catch (Exception ex)
            {
                var containerException = new ImportFailedException(fileHeader, ex);
                Logger.Error(containerException);
                throw containerException;
            }
        }

        /// <summary>
        ///     Gets the key of the imported file.
        /// </summary>
        /// <param name="fileHeader">The header describing the imported file.</param>
        /// <param name="locationKey">
        ///     The key that uniquely identifies the target location
        ///     for the content to be imported.
        /// </param>
        /// <returns>A key, that uniquely identifies the imported file.</returns>
        protected abstract string GetImportKey(TFileHeader fileHeader, string locationKey);

        /// <summary>
        ///     Restores the file on the targeted file system.
        /// </summary>
        /// <param name="importKey">The key, that uniquely identifies the imported file.</param>
        /// <param name="fileHeader">The FileHeader, which describes the contained file.</param>
        /// <param name="body">The MigrationContainer's body.</param>
        protected abstract void RestoreFile(TFileHeader fileHeader, IContainerBody body, string importKey);
    }
}
