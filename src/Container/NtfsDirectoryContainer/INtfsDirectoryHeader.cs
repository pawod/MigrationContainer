namespace DataMigrator.Container.NtfsDirectoryContainer
{
    using System.IO;
    using Base.Header;
    using DirectoryContainer.Base;
    using FileContainer.Header;

    public interface INtfsDirectoryHeader : INtfsFilesystemHeader<DirectoryInfo>,
                                            IDirectoryHeaderBase<NtfsDirectoryHeader, NtfsFileHeader>
    {
    }
}
