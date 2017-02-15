using Pawod.MigrationContainer.Container.Header.Base;

namespace Pawod.MigrationContainer.Container.Enumerator.Mapping
{
    public interface IFilesystemMapping<out THeader>
        where THeader : IProtoHeader
    {
        THeader Header { get; }
        string SourceName { get; }
    }
}