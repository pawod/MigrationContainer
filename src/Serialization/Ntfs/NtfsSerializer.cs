namespace DataMigrator.Serialization.Ntfs
{
    using System.IO;
    using Base;
    using CodeFluent.Runtime.BinaryServices;
    using Container.Base.Header;

    public class NtfsSerializer<TContentHeader, TFsInfo> : ContainerSerializer<TContentHeader, TFsInfo>
        where TContentHeader : INtfsFilesystemHeader<TFsInfo>
        where TFsInfo : FileSystemInfo
    {
	    public NtfsSerializer(int contentBufferSize) : base(contentBufferSize)
	    {
	    }

	    protected override Stream OpenSourceStream(string streamName)
        {
            return streamName.Contains(":")
                ? NtfsAlternateStream.Open(streamName, FileAccess.Read, FileMode.Open, FileShare.None)
                : base.OpenSourceStream(streamName);
        }
    }
}
