using Pawod.MigrationContainer.Container.Header.Base;

namespace Pawod.MigrationContainer.Container.Enumerator.Mapping
{
    // ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

    /// <summary>
    ///     Associates a FileHeader with an enitre directory or a binary file on filesystem.
    /// </summary>
    public class FilesystemMapping<THeader> : IFilesystemMapping<THeader>
        where THeader : IProtoHeader
    {
        public FilesystemMapping(string sourceName, THeader header)
        {
            Header = header;
            SourceName = sourceName;
        }

        public THeader Header { get; private set; }
        public string SourceName { get; private set; }
    }
}