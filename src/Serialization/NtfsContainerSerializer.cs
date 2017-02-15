using System.IO;
using Alphaleonis.Win32.Filesystem;
using Pawod.MigrationContainer.Container.Header.NTFS;
using Pawod.MigrationContainer.Filesystem.NTFS;
using Pawod.MigrationContainer.Serialization.Partitioning;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Pawod.MigrationContainer.Serialization
{
    public class NtfsContainerSerializer<THeader, TSource, TExport> : ContainerSerializer<THeader, TSource, TExport>
        where TSource : INtfsFile
        where TExport : INtfsFile
        where THeader : INtfsFileHeader
    {
        public NtfsContainerSerializer(int contentBufferSize) : base(contentBufferSize)
        {
        }

        protected override Stream GetSourceStream(IPartitionInfo partitionInfo)
        {
            var path = Path.GetLongPath(partitionInfo.ContentStreamId);
            return File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read, ExtendedFileAttributes.BackupSemantics, PathFormat.LongFullPath);
        }
    }
}