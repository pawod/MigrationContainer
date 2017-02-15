using System.Collections.Generic;
using System.IO;
using System.Linq;
using Pawod.MigrationContainer.Container.Enumerator.Filesystem;

namespace Pawod.MigrationContainer.Filesystem.Base
{
    /// <summary>
    ///     An abstractopm of a directory on a file system.
    /// </summary>
    public abstract class Directory<TFile, TDirectory> : File<TFile, TDirectory>, IDirectory
        where TFile : File<TFile, TDirectory>
        where TDirectory : Directory<TFile, TDirectory>
    {
        public abstract IList<IFile> GetFiles(string pattern, SearchOption searchOption);
        public abstract IList<IFile> GetFiles();

        public override bool IsMigrationContainer()
        {
            return false;
        }

        IList<IDirectory> IDirectory.GetDirectories()
        {
            return GetDirectories().ToList<IDirectory>();
        }

        IFileSystemEnumerator<IDirectory, IFile> IDirectory.GetEnumerator(IList<string> filter)
        {
            return GetEnumerator(filter);
        }

        IFileSystemEnumerator<IDirectory, IFile> IDirectory.GetEnumerator()
        {
            return GetEnumerator();
        }

        public abstract IList<TDirectory> GetDirectories();
        public abstract IFileSystemEnumerator<TDirectory, TFile> GetEnumerator(IList<string> filter);
        public abstract IFileSystemEnumerator<TDirectory, TFile> GetEnumerator();
    }
}