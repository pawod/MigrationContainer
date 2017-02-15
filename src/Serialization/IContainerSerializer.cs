using System;
using Pawod.MigrationContainer.Container.Header.Base;
using Pawod.MigrationContainer.Filesystem.Base;
using Pawod.MigrationContainer.Serialization.Parameters;

namespace Pawod.MigrationContainer.Serialization
{
    public interface IContainerSerializer<THeader, TSource, TExport> : IDisposable
        where THeader : IFileHeader
        where TSource : IFile
        where TExport : IFile
    {
        TExport Serialize(ISerializationParameters<THeader, TSource, TExport> parameters, int part);
    }
}