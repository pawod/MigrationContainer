using System;
using System.Collections;
using System.Collections.Generic;
using Pawod.MigrationContainer.Container.Enumerator.Filesystem;
using Pawod.MigrationContainer.Container.Enumerator.Header;
using Pawod.MigrationContainer.Container.Header.Base;
using Pawod.MigrationContainer.Filesystem.Base;

namespace Pawod.MigrationContainer.Container.Enumerator.Mapping
{
    /// <summary>
    ///     Enumerates a FilesystemMapping's headers simultaneously with its associated content.
    /// </summary>
    public abstract class FilesystemMappingEnumerator<TDirectoryHeader, TFileHeader, TNode, TFile, TDirectory> :
        IEnumerator<IFilesystemMapping<IFileHeader>>
        where TDirectoryHeader : class, IDirectoryHeader<TDirectoryHeader, TFileHeader>
        where TFileHeader : class, IFileHeader
        where TNode : FileSystemTreeNode<TDirectory, TFile>
        where TFile : IFile
        where TDirectory : IDirectory

    {
        protected IFilesystemMapping<IFileHeader> CurrentFilesystemMapping;
        protected readonly IDirectoryHeaderEnumerator<TDirectoryHeader, TFileHeader> DirHeaderEnumerator;
        private readonly FileSystemEnumerator<TNode, TDirectory, TFile> _directoryEnumerator;

        protected IFileHeader CurrentHeader
            => DirHeaderEnumerator.Current.IsNode() ? DirHeaderEnumerator.Current.Node : (IFileHeader) DirHeaderEnumerator.Current.Leaf;

        protected string CurrentSourceName
            => _directoryEnumerator.Current.IsNode() ? _directoryEnumerator.Current.Node.FullPath : _directoryEnumerator.Current.Leaf.FullPath;

        protected FilesystemMappingEnumerator(IDirectory rootDirectory, TDirectoryHeader rootDirectoryHeader, IList<string> filter = null)
        {
            _directoryEnumerator = (FileSystemEnumerator<TNode, TDirectory, TFile>) rootDirectory.GetEnumerator(filter);
            DirHeaderEnumerator = rootDirectoryHeader.GetEnumerator(filter);
        }

        public IFilesystemMapping<IFileHeader> Current => CurrentFilesystemMapping;

        public void Dispose()
        {
        }

        public abstract bool MoveNext();

        public void Reset()
        {
            DirHeaderEnumerator.Reset();
            _directoryEnumerator.Reset();
        }

        object IEnumerator.Current => Current;

        protected bool BasicMoveNext()
        {
            {
                var result1 = _directoryEnumerator.MoveNext();
                var result2 = DirHeaderEnumerator.MoveNext();

                if (result1 != result2)
                {
                    throw new ApplicationException(
                        "FileHeader does not match directory's tree structure. Either the FileHeader is faulty or the FilesystemEnumerator's root does not match FileHeader's root.");
                }

                CurrentFilesystemMapping = new FilesystemMapping<IFileHeader>(CurrentSourceName, CurrentHeader);
                return result1;
            }
        }
    }
}