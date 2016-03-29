namespace DataMigrator.Container.Base.Header
{
	using Helper;
	// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
	using System.Collections.Generic;
    using System.IO;
    using DirectoryContainer.Base;
    using DirectoryContainer.Header;
    using FileContainer.Header;
    using NtfsDirectoryContainer;
    using ProtoBuf;

    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    [ProtoInclude(7, typeof(DirectoryHeaderBase<DirectoryHeader, FileHeader>))]
    [ProtoInclude(8, typeof(DirectoryHeaderBase<NtfsDirectoryHeader, NtfsFileHeader>))]
    [ProtoInclude(9, typeof(FileHeader))]
    public abstract class FilesystemHeader<TFsInfo> : ProtoHeader, IFilesystemHeader<TFsInfo>
        where TFsInfo : FileSystemInfo
    {
        public long ContentLength { get; protected set; }
        public long ContentOffset { get; private set; }
        public string OriginalName { get; set; }
        public IDictionary<string, long> TimeStamps { get; private set; }

        protected FilesystemHeader(long contentOffset, long nextHeaderLength)
        {
            ContentOffset = contentOffset;
            NextHeaderLength = nextHeaderLength;
        }

        protected FilesystemHeader()
        {
        }

        public virtual void AssociateWith(TFsInfo fsInfo, IList<string> filter = null)
        {
            OriginalName = fsInfo.Name;
            TimeStamps = GetTimestamps(fsInfo);
        }

        protected static Dictionary<string, long> GetTimestamps(TFsInfo fsInfo)
        {
            return new Dictionary<string, long> {
                                                    {"CreationTimeUtc", fsInfo.CreationTimeUtc.UtcDateTimeToUnixTimeStamp()},
                                                    {"LastAccessTimeUtc", fsInfo.LastAccessTimeUtc.UtcDateTimeToUnixTimeStamp()},
                                                    {"LastWriteTimeUtc", fsInfo.LastWriteTimeUtc.UtcDateTimeToUnixTimeStamp()}
                                                };
        }
    }
}
