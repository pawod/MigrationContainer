namespace DataMigrator.Serialization.Base
{
    using System;
    using System.IO;
    using Container.Base.Header;
    using Parameters.Base;

    public interface IContainerSerializer<in TContentHeader, in TFsInfo> : IDisposable
        where TContentHeader : IFilesystemHeader<TFsInfo>
        where TFsInfo : FileSystemInfo
    {
        FileInfo Serialize(ISerializationParameters<TContentHeader, TFsInfo> parameters, int part);
    }
}
