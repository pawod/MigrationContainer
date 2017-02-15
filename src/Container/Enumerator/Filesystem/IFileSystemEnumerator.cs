using Pawod.MigrationContainer.Container.Enumerator.Base;
using Pawod.MigrationContainer.Filesystem.Base;

namespace Pawod.MigrationContainer.Container.Enumerator.Filesystem
{
    public interface IFileSystemEnumerator<out TDirectory, out TFile> : ITreeEnumerator<TDirectory, TFile>
        where TDirectory : IDirectory
        where TFile : IFile
    {
    }
}