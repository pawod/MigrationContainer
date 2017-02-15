using System.Collections.Generic;
using Pawod.MigrationContainer.Container.Enumerator.Base;
using Pawod.MigrationContainer.Container.Header.Base;

namespace Pawod.MigrationContainer.Container.Enumerator.Header
{
    public class DirectoryHeaderEnumerator<TNodeWrapper, TDirHeader, TFileHeader> : TreeEnumerator<TNodeWrapper, TDirHeader, TFileHeader>,
                                                                                    IDirectoryHeaderEnumerator<TDirHeader, TFileHeader>
        where TDirHeader : class, IDirectoryHeader<TDirHeader, TFileHeader>
        where TFileHeader : class, IFileHeader
        where TNodeWrapper : DirectoryHeaderTreeNode<TDirHeader, TFileHeader>
    {
        public DirectoryHeaderEnumerator(TDirHeader root, IList<string> filter = null) : base(root, filter)
        {
        }
    }
}