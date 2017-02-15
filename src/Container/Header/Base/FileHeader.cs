using Pawod.MigrationContainer.Container.Header.NTFS;
using Pawod.MigrationContainer.Extensions;
using Pawod.MigrationContainer.Filesystem.Base;
using Pawod.MigrationContainer.Filesystem.NTFS;
using ProtoBuf;

namespace Pawod.MigrationContainer.Container.Header.Base
{
    /// <summary>
    ///     Represents the base class for all file system headers.
    /// </summary>
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    [ProtoInclude(7, typeof(NtfsFileHeader))]
    [ProtoInclude(8, typeof(DirectoryHeader<NtfsDirectoryHeader, NtfsFileHeader, NtfsFile, NtfsDirectory>))]
    public abstract class FileHeader : ProtoHeader, IFileHeader
    {
        public long TimeCreatedUtc { get; set; }
        public long TimeModifiedUtc { get; set; }

        protected FileHeader(IFile file, long contentOffset, long nextHeaderLength)
        {
            OriginalName = file.Name;
            TimeCreatedUtc = file.DateCreatedUtc.UtcDateTimeToUnixTimeStamp();
            TimeModifiedUtc = file.DateLastModifiedUtc.UtcDateTimeToUnixTimeStamp();

            ContentLength = file.Length;
            ContentOffset = contentOffset;
            NextHeaderLength = nextHeaderLength;
        }

        protected FileHeader()
        {
        }

        public long ContentLength { get; protected set; }
        public long ContentOffset { get; protected set; }
        public string OriginalName { get; set; }
    }
}