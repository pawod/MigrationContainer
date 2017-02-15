using Pawod.MigrationContainer.Container.Enumerator.Base;
using Pawod.MigrationContainer.Container.Header.Base;

namespace Pawod.MigrationContainer.Container.Enumerator.Header
{
    public interface IDirectoryHeaderEnumerator<out TDirHeader, out TFileHeader> : ITreeEnumerator<TDirHeader, TFileHeader>
        where TDirHeader : class, IDirectoryHeader<TDirHeader, TFileHeader>
        where TFileHeader : class, IFileHeader
    {
    }
}