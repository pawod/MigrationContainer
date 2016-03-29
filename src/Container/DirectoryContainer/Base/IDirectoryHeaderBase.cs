namespace DataMigrator.Container.DirectoryContainer.Base
{
    using System.Collections.Generic;
    using System.IO;
    using Container.Base.Header;
    using Enumerator.Base;

    public interface IDirectoryHeaderBase<TDirectoryHeader, TFileHeader> : IFilesystemHeader<DirectoryInfo>
        where TDirectoryHeader : class, IDirectoryHeaderBase<TDirectoryHeader, TFileHeader>
        where TFileHeader : IFilesystemHeader<FileInfo>
    {
        IList<TFileHeader> Files { get; }
        string RelativePath { get; set; }
        IList<TDirectoryHeader> SubDirectories { get; }
        IEnumerator<ITreeElement<TDirectoryHeader, TFileHeader>> GetEnumerator();
		IEnumerator<ITreeElement<TDirectoryHeader, TFileHeader>> GetEnumerator(IList<string> filter);
        void InitializeFrom(DirectoryInfo directoryInfo, IList<string> filter = null);
    }
}
