namespace DataMigrator.Adapters.Base
{
    using System.IO;
    using Container.Base;
    using Container.Base.Header;

    /// <summary>
    ///     Provides basic functionality for the import of a container's content.
    /// </summary>
    /// <typeparam name="TContainer">
    ///     The type of MigrationContainer, which holds the
    ///     content to be imported.
    /// </typeparam>
    /// <typeparam name="TContentHeader">
    ///     The type of ContentHeader associated with the
    ///     content to be imported.
    /// </typeparam>
    /// <typeparam name="TFsInfo">
    ///     The type of content stored inside the container's body.
    ///     Files or directories.
    /// </typeparam>
    public interface IContentAdapter<in TContainer, in TContentHeader, out TFsInfo>
        where TContainer : MigrationContainerInfo<TContainer, TContentHeader, TFsInfo>
        where TContentHeader : class, IFilesystemHeader<TFsInfo>
        where TFsInfo : FileSystemInfo

    {
        /// <summary>
        ///     Imports a MigrationContainer's content to the target environment.
        /// </summary>
        /// <param name="container">The container to be imported.</param>
        /// <param name="locationKey">
        ///     The key that uniquely identifies the target location for
        ///     the content to be imported.
        /// </param>
        /// <returns>The key, that uniquely identifies the imported content.</returns>
        string Import(TContainer container, string locationKey);
    }
}
