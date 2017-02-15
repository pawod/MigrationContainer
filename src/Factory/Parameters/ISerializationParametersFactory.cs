using Pawod.MigrationContainer.Container.Header.Base;
using Pawod.MigrationContainer.Filesystem.Base;
using Pawod.MigrationContainer.Serialization.Parameters;

namespace Pawod.MigrationContainer.Factory.Parameters
{
    public interface ISerializationParametersFactory<THeader, TSource, out TExport>
        where THeader : class, IFileHeader
        where TSource : IFile
        where TExport : IFile
    {
        ISerializationParameters<THeader, TSource, TExport> Create(TSource source, IDirectory targetDir, params IProtoHeader[] appHeaders);
    }
}