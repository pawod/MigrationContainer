using Alphaleonis.Win32.Filesystem;
using Pawod.MigrationContainer.Container.Attribute;
using Pawod.MigrationContainer.Container.Body;
using Pawod.MigrationContainer.Container.Header.NTFS;
using Pawod.MigrationContainer.Filesystem.NTFS;

namespace Pawod.MigrationContainer.Container
{
    [ContainerMetaDescription(".dc")]
    public class NtfsDirectoryContainer : MigrationContainer<NtfsDirectoryContainer, NtfsDirectoryHeader, ContainerBody>
    {
        public NtfsDirectoryContainer(FileInfo containerFile) : base(new NtfsFile(containerFile))
        {
        }

        public NtfsDirectoryContainer(NtfsFile containerFile) : base(containerFile)
        {
        }
    }
}