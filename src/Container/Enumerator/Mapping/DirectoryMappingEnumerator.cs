using System.Collections.Generic;
using Pawod.MigrationContainer.Container.Enumerator.Filesystem;
using Pawod.MigrationContainer.Container.Header.Base;
using Pawod.MigrationContainer.Filesystem.Base;

namespace Pawod.MigrationContainer.Container.Enumerator.Mapping
{
    public class DirectoryMappingEnumerator<TDirectoryHeader, TFileHeader, TNode, TFile, TDirectory> :
        FilesystemMappingEnumerator<TDirectoryHeader, TFileHeader, TNode, TFile, TDirectory>
        where TDirectoryHeader : DirectoryHeader<TDirectoryHeader, TFileHeader, TFile, TDirectory>
        where TFileHeader : class, IFileHeader
        where TNode : FileSystemTreeNode<TDirectory, TFile>
        where TFile : IFile
        where TDirectory : IDirectory


    {
        public DirectoryMappingEnumerator(TDirectory rootDirectory, TDirectoryHeader rootDirectoryHeader, IList<string> filter = null)
            : base(rootDirectory, rootDirectoryHeader, filter)
        {
        }

        public override bool MoveNext()
        {
            if (!BasicMoveNext()) return false;
            while (DirHeaderEnumerator.Current.IsNode())
            {
                if (!BasicMoveNext()) return false;
            }
            return true;
        }
    }
}