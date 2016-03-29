namespace DataMigrator.Container.FileContainer
{
    using System.IO;
    using Base;
    using Header;

    [ContainerMetaDescription(".xfc")]
    public class NtfsFileContainerInfo : FileContainerInfoBase<NtfsFileContainerInfo, NtfsFileHeader>
    {
        public NtfsFileContainerInfo(FileInfo fileInfo) : base(fileInfo)
        {
        }
    }
}
