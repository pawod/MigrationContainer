namespace DataMigrator.Enumerator.ContentMapping
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using Base;
    using Container.Base.Header;
    using Container.DirectoryContainer.Base;
    using Filesystem;
    using Helper;

    /// <summary>
    ///     Enumerates a ContentHeader simultaneously with its associated directory.
    /// </summary>
    public abstract class ContentMappingEnumerator<TDirectoryHeader, TFileHeader> :
        IEnumerator<IHeaderSourceMapping<IContentHeader>>
        where TDirectoryHeader : class, IDirectoryHeaderBase<TDirectoryHeader, TFileHeader>
        where TFileHeader : class, IFilesystemHeader<FileInfo>
    {
        private readonly FilesystemEnumerator _directoryEnumerator;
        protected readonly IEnumerator<ITreeElement<TDirectoryHeader, TFileHeader>> ContentHeaderEnumerator;
        protected IHeaderSourceMapping<IContentHeader> CurrentHeaderSourceMapping;

        public IHeaderSourceMapping<IContentHeader> Current
        {
            get { return CurrentHeaderSourceMapping; }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        protected IContentHeader CurrentHeader
        {
            get
            {
                return ContentHeaderEnumerator.Current.IsNode()
                    ? ContentHeaderEnumerator.Current.Node
                    : (IContentHeader)ContentHeaderEnumerator.Current.Leaf;
            }
        }

        protected string CurrentSourceName
        {
            get
            {
                return _directoryEnumerator.Current.IsNode()
                    ? _directoryEnumerator.Current.Node.FullName
                    : _directoryEnumerator.Current.Leaf.FullName;
            }
        }

        protected ContentMappingEnumerator(DirectoryInfo rootDirectory, TDirectoryHeader rootDirectoryHeader, IList<string> filter = null)
        {
            _directoryEnumerator = rootDirectory.GetEnumerator(filter);
            ContentHeaderEnumerator = rootDirectoryHeader.GetEnumerator(filter);
        }

        public void Dispose()
        {
        }

        public abstract bool MoveNext();

        public void Reset()
        {
            ContentHeaderEnumerator.Reset();
            _directoryEnumerator.Reset();
        }

        protected bool BasicMoveNext()
        {
            {
                var result1 = _directoryEnumerator.MoveNext();
                var result2 = ContentHeaderEnumerator.MoveNext();

                if (result1 != result2)
                {
                    throw new ApplicationException(
                        "ContentHeader does not match directory's tree structure. Either the ContentHeader is faulty or the FilesystemEnumerator's root does not match ContentHeader's root.");
                }

                CurrentHeaderSourceMapping = new HeaderSourceMapping<IContentHeader>(CurrentSourceName, CurrentHeader);
                return result1;
            }
        }
    }
}
