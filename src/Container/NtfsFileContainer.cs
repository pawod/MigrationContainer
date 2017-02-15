using Alphaleonis.Win32.Filesystem;
using Pawod.MigrationContainer.Container.Attribute;
using Pawod.MigrationContainer.Container.Body;
using Pawod.MigrationContainer.Container.Header.NTFS;
using Pawod.MigrationContainer.Filesystem.NTFS;

namespace Pawod.MigrationContainer.Container
{
    [ContainerMetaDescription(".fc")]
    public class NtfsFileContainer : MigrationContainer<NtfsFileContainer, NtfsFileHeader, ContainerBody>
    {
        public NtfsFileContainer(FileInfo containerFile) : base(new NtfsFile(containerFile))
        {
        }

        public NtfsFileContainer(NtfsFile containerFile) : base(containerFile)
        {
        }
    }
}