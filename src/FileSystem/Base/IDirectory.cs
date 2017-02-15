using System.Collections.Generic;
using System.IO;
using Pawod.MigrationContainer.Container.Enumerator.Filesystem;

namespace Pawod.MigrationContainer.Filesystem.Base
{
    public interface IDirectory : IFile
    {
        IList<IDirectory> GetDirectories();
        IFileSystemEnumerator<IDirectory, IFile> GetEnumerator(IList<string> filter);
        IFileSystemEnumerator<IDirectory, IFile> GetEnumerator();
        IList<IFile> GetFiles(string pattern, SearchOption searchOption);
        IList<IFile> GetFiles();
    }
}