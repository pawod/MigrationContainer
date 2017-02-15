using System.Collections.Generic;
using Pawod.MigrationContainer.Container.Enumerator.Base;
using Pawod.MigrationContainer.Filesystem.Base;

namespace Pawod.MigrationContainer.Container.Enumerator.Filesystem
{
    public class FileSystemEnumerator<TNodeWrapper, TDirectory, TFile> : TreeEnumerator<TNodeWrapper, TDirectory, TFile>,
                                                                         IFileSystemEnumerator<TDirectory, TFile>
        where TDirectory : IDirectory
        where TFile : IFile
        where TNodeWrapper : FileSystemTreeNode<TDirectory, TFile>
    {
        public FileSystemEnumerator(TDirectory root, IList<string> filter = null) : base(root, filter)
        {
        }
    }
}