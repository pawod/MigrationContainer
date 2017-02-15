using Pawod.MigrationContainer.Container.Header.Base;

namespace Pawod.MigrationContainer.Container.Header.NTFS
{
    public interface INtfsDirectoryHeader<TChild, TFileHeader> : IDirectoryHeader<TChild, TFileHeader>, INtfsFileHeader
        where TChild : class, IDirectoryHeader<TChild, TFileHeader>
        where TFileHeader : class, IFileHeader
    {
    }
}