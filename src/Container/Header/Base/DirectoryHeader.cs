using System;
using System.Collections.Generic;
using System.IO;
using Pawod.MigrationContainer.Container.Enumerator.Header;
using Pawod.MigrationContainer.Container.Header.NTFS;
using Pawod.MigrationContainer.Filesystem.Base;
using ProtoBuf;

namespace Pawod.MigrationContainer.Container.Header.Base
{
    /// <summary>
    ///     Represents the standard header for typical hierarchic filesystems.
    /// </summary>
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    [ProtoInclude(6, typeof(NtfsDirectoryHeader))]
    public abstract class DirectoryHeader<TDirectoryHeader, TFileHeader, TFile, TDirectory> : FileHeader,
                                                                                              IDirectoryHeader<TDirectoryHeader, TFileHeader>
        where TDirectoryHeader : DirectoryHeader<TDirectoryHeader, TFileHeader, TFile, TDirectory>
        where TFileHeader : class, IFileHeader
        where TFile : IFile
        where TDirectory : IDirectory
    {
        protected DirectoryHeader()
        {
        }

        protected DirectoryHeader(TDirectory directory, long contentOffset, long nextHeaderLength, string parentPath = null)
            : base(directory, contentOffset, nextHeaderLength)
        {
            RelativePath = Path.Combine(parentPath ?? string.Empty, OriginalName);
            ContentLength = 0; // needs to be reset, since base class does not consider filters
        }

        public IList<TFileHeader> FileHeaders { get; set; } = new List<TFileHeader>();

        public IList<TDirectoryHeader> GetDirectoryHeaders()
        {
            var enumerator = GetEnumerator();
            var list = new List<TDirectoryHeader>();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.IsNode()) list.Add(enumerator.Current.Node);
            }
            return list;
        }

        public abstract IDirectoryHeaderEnumerator<TDirectoryHeader, TFileHeader> GetEnumerator();
        public abstract IDirectoryHeaderEnumerator<TDirectoryHeader, TFileHeader> GetEnumerator(IList<string> filter);


        public IList<TFileHeader> GetFileHeaders()
        {
            var enumerator = GetEnumerator();
            var list = new List<TFileHeader>();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.IsLeaf()) list.Add(enumerator.Current.Leaf);
            }
            return list;
        }

        public string RelativePath { get; set; }
        public IList<TDirectoryHeader> SubdirHeaders { get; set; } = new List<TDirectoryHeader>();

        /// <summary>
        ///     Adds all necessary headers to this header of a directory to map all content (files and subdirs) properly.
        /// </summary>
        /// <param name="dirInfo">The directory whose content is to be mapped.</param>
        /// <param name="nameFilter">Subdirs and files with a matching name, that should not be mapped.</param>
        public void MapDirectoryContent(TDirectory dirInfo, IList<string> nameFilter = null)
        {
            var offsetAkk = ContentOffset;
            MapDirectoryContentRecursive(dirInfo, nameFilter, ref offsetAkk);
        }

        /// <summary>
        ///     Creates a new FileHeader, which is mapped to the specified file.
        /// </summary>
        /// <param name="file">The file to be mapped.</param>
        /// <param name="contentOffset">
        ///     The amount of bytes already used by this DirectoryHeader's contents. It describes the content's position relative
        ///     to the body's starting position (partitioning overhead not considered). The value of this parameter is updated by
        ///     the content size of the created FileHeader. This value represents the ContentOffset for succeeding ContentHeaders.
        /// </param>
        /// <returns>A FileHeader, ready to be added to this DirectoryHeader.</returns>
        protected virtual TFileHeader CreateFileHeader(TFile file, ref long contentOffset)
        {
            // NextHeaderLength is not relevant to nested ContentHeaders
            var fileHeader = (TFileHeader) Activator.CreateInstance(typeof(TFileHeader), file, contentOffset, 0);
            contentOffset += fileHeader.ContentLength;
            return fileHeader;
        }

        /// <summary>
        ///     Creates a new DirectoryHeader, which is mapped to the specified directory.
        /// </summary>
        /// <param name="directory">The Directory to be mapped.</param>
        /// <param name="contentOffset">
        ///     The amount of bytes already used by this DirectoryHeader's contents. It describes the content's position relative
        ///     to the body's starting position (partitioning overhead not considered). The value of this parameter is updated by
        ///     the content size of the created DirectoryHeader. This value represents the ContentOffset for succeeding
        ///     ContentHeaders.
        /// </param>
        /// <param name="nameFilter">Subdirs and files with a matching name, that should not be mapped.</param>
        /// <returns>A DirectoryHeader, ready to be added to this DirectoryHeader.</returns>
        protected virtual TDirectoryHeader CreateSubDirHeader(TDirectory directory, ref long contentOffset, IList<string> nameFilter = null)
        {
            // NextHeaderLength is not relevant to nested ContentHeaders
            var dirHeader = (TDirectoryHeader) Activator.CreateInstance(typeof(TDirectoryHeader), directory, contentOffset, 0, RelativePath);
            dirHeader.MapDirectoryContentRecursive(directory, nameFilter, ref contentOffset);
            return dirHeader;
        }

        /// <summary>
        ///     Adds all necessary headers to this header of a subdirectory to map all content (files and subdirs) properly.
        /// </summary>
        /// <param name="directory">The directory whose content is to be mapped.</param>
        /// <param name="nameFilter">Subdirs and files with a matching name, that should not be mapped.</param>
        /// <param name="offsetAkk">
        ///     Keeps track of the required ContentOffset when new content is added to the container. Pass the DirectoryHeader's
        ///     current ContentLength as offset for subsequent ContentHeaders.
        /// </param>
        protected void MapDirectoryContentRecursive(TDirectory directory, IList<string> nameFilter, ref long offsetAkk)
        {
            if (!MatchesMappingConstraints(directory, nameFilter)) return;

            foreach (var file in directory.GetFiles())
            {
                if (!MatchesMappingConstraints((TFile) file, nameFilter)) continue;
                var fileHeader = CreateFileHeader((TFile) file, ref offsetAkk);
                FileHeaders.Add(fileHeader);
                ContentLength += fileHeader.ContentLength;
            }

            foreach (var subDir in directory.GetDirectories())
            {
                if (!MatchesMappingConstraints((TDirectory) subDir, nameFilter)) continue;
                var subdDirHeader = CreateSubDirHeader((TDirectory) subDir, ref offsetAkk, nameFilter);
                SubdirHeaders.Add(subdDirHeader);
                ContentLength += subdDirHeader.ContentLength;
            }
        }

        /// <summary>
        ///     Determines wether this directory should be mapped or not. The base implementation excludes all directories from
        ///     mapping whose name is contained in the specified name filter list. Override this method to add further constraints
        ///     besides the name filter.
        /// </summary>
        /// <param name="directory">The directory to be inspected.</param>
        /// <param name="nameFilter">Subdirs and files with a matching name, that should not be mapped.</param>
        /// <returns>
        ///     Return true if this directory should be mapped to a DirectoryHeader. Else false.
        /// </returns>
        protected virtual bool MatchesMappingConstraints(TDirectory directory, IList<string> nameFilter)
        {
            return nameFilter == null || !nameFilter.Contains(directory.Name);
        }

        /// <summary>
        ///     Determines wether this file should be mapped or not. The base implementation excludes all files from
        ///     mapping whose name is contained in the specified name filter list. Override this method to add further constraints
        ///     besides the name filter.
        /// </summary>
        /// <param name="file">The file to be inspected.</param>
        /// <param name="nameFilter">Subdirs and files with a matching name, that should not be mapped.</param>
        /// <returns>
        ///     Return true if this file should be mapped to a FileHeader. Else false.
        /// </returns>
        protected virtual bool MatchesMappingConstraints(TFile file, IList<string> nameFilter)
        {
            return nameFilter == null || !nameFilter.Contains(file.Name);
        }
    }
}