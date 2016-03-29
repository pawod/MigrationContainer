namespace DataMigrator.Adapters.NTFS.Directory
{
    using Container.NtfsDirectoryContainer;

    public interface INtfsDirectoryAdapter : INtfsAdapter
    {
        /// <summary>
        ///     Imports the a JunctionPoint based on the specified JunctionHeader.
        /// </summary>
        /// <param name="junctionHeader">
        ///     The JunctionHeader, that describes the junction to
        ///     be imported.
        /// </param>
        /// <param name="targetPath">The path to the location, where to create the junction.</param>
        void ImportJunction(JunctionHeader junctionHeader, string targetPath);
    }
}
