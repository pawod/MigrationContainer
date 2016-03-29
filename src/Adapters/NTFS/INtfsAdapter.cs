namespace DataMigrator.Adapters.NTFS
{
    using Container.Base.Body;
    using Container.Base.Header;
    using Container.FileContainer.Header;

    public interface INtfsAdapter
    {
        /// <summary>
        ///     Imports an alternate stream, based on the header, that describes it.
        /// </summary>
        /// <param name="body">The MigrationContainer's body.</param>
        /// <param name="alternateStreamHeader">
        ///     The header, which describes the alternate
        ///     stream to be imported.
        /// </param>
        /// <param name="streamTargetPath">
        ///     The path to the location, where to import the
        ///     alternate stream.
        /// </param>
        void ImportAlternateStream(IContainerBody body,
                                   AlternateStreamHeader alternateStreamHeader,
                                   string streamTargetPath);
    }
}
