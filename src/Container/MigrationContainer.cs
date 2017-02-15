using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Pawod.MigrationContainer.Configuration;
using Pawod.MigrationContainer.Container.Body;
using Pawod.MigrationContainer.Container.Header.Base;
using Pawod.MigrationContainer.Exception;
using Pawod.MigrationContainer.Extensions;
using Pawod.MigrationContainer.Filesystem.Base;

namespace Pawod.MigrationContainer.Container
{
    /// <summary>
    ///     The base class of migration containers. It provides access to the serialized
    ///     container's headers and its content.
    /// </summary>
    public abstract class MigrationContainer<TChild, THeader, TBody> : IMigrationContainer
        where TChild : MigrationContainer<TChild, THeader, TBody>
        where THeader : class, IFileHeader
        where TBody : IContainerBody
    {
        private TBody _body;
        private THeader _header;
        private StartHeader _startHeader;


        public THeader ContentHeader
        {
            get
            {
                if (_header != null) return _header;
                var mainPart = IsMainPart() ? this : FindMainPart();
                if (mainPart == null) throw new MainPartNotFoundException(File.Parent.FullPath);
                return _header = ProtoHeader.Get<THeader>(mainPart.File, mainPart.StartHeader, Configuration.ContentBufferSize);
            }
        }


        public virtual StartHeader StartHeader => _startHeader ?? (_startHeader = StartHeader.Extract(File));

        protected ContainerConfiguration Configuration => ContainerConfiguration.Instance(GetType());

        protected MigrationContainer(IFile file)
        {
            File = file;
            Directory = file.Parent;
        }

        public IContainerBody Body
        {
            get
            {
                if (_body == null)
                {
                    var allParts = GetAllParts();
                    if (StartHeader.Parts != allParts.Count) throw new MissingPartException(StartHeader.Parts, allParts.Count);
                    _body = CreateBodyInstance(allParts);
                }
                return _body;
            }
        }

        public virtual long BodyPosition => StartHeader.Length + StartHeader.NextHeaderLength;

        public IDirectory Directory { get; }

        public IFile File { get; }

        public bool IsLastPart()
        {
            return StartHeader.PartNumber == StartHeader.Parts;
        }

        public bool IsMainPart()
        {
            return StartHeader.PartNumber == 0;
        }

        public bool IsParted()
        {
            return StartHeader.Parts > 1;
        }

        public bool IsValid()
        {
            using (var md5 = MD5.Create())
            using (var fileStream = File.OpenRead())
            {
                // skip magic numbers and checksum
                fileStream.Position = StartHeader.MagicNumbers.Count + StartHeader.MD5_LENGTH;

                var hash = md5.ComputeHash(fileStream);
                return StartHeader.Md5Hash.SequenceEqual(hash);
            }
        }

        IFileHeader IMigrationContainer.ContentHeader => ContentHeader;

        IMigrationContainer IMigrationContainer.FindMainPart(SearchOption searchOption, bool ignoreFileExtension)
        {
            return FindMainPart(searchOption, ignoreFileExtension);
        }

        IEnumerable<IMigrationContainer> IMigrationContainer.FindRelatedParts(SearchOption searchOption, bool ignoreFileExtension)
        {
            return FindRelatedParts(searchOption, ignoreFileExtension);
        }

        IEnumerable<IFile> IMigrationContainer.GetAllParts(SearchOption searchOption)
        {
            return GetAllParts(searchOption);
        }

        IStartHeader IMigrationContainer.StartHeader => StartHeader;


        public TChild FindMainPart(SearchOption searchOption = SearchOption.TopDirectoryOnly, bool ignoreFileExtension = false)
        {
            if (IsMainPart()) return (TChild) this;
            var pattern = ignoreFileExtension ? "*" : $"*{GetType().GetFileExtension()}";
            var containers =
                Directory.GetFiles(pattern, searchOption)
                         .Where(file => file.IsMigrationContainer())
                         .Select(file => (TChild) Activator.CreateInstance(typeof(TChild), file))
                         .ToArray();
            return !containers.Any() ? null : containers.FirstOrDefault(c => c.IsMainPart() && (c.StartHeader.ContainerId == StartHeader.ContainerId));
        }


        public IList<TChild> FindRelatedParts(SearchOption searchOption = SearchOption.TopDirectoryOnly, bool ignoreFileExtension = false)
        {
            if (!File.IsMigrationContainer()) throw new InvalidContainerException();

            var pattern = ignoreFileExtension ? "*" : $"*{GetType().GetFileExtension()}.part*";
            var relatedParts =
                Directory.GetFiles(pattern, searchOption)
                         .Where(file => file.IsMigrationContainer())
                         .Select(file => (TChild) Activator.CreateInstance(typeof(TChild), file))
                         .ToList();

            return !relatedParts.Any() ? relatedParts : relatedParts.Where(c => c.StartHeader.ContainerId == StartHeader.ContainerId).ToList();
        }


        public List<IFile> GetAllParts(SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var allParts = new List<IFile>();
            var mainPart = FindMainPart(searchOption).File;
            if (mainPart != null) allParts.Add(mainPart);
            var relatedParts = FindRelatedParts(searchOption);
            if (relatedParts != null) allParts.AddRange(relatedParts.Select(c => c.File).OrderBy(p => p.Name));
            return allParts;
        }

        protected virtual TBody CreateBodyInstance(List<IFile> allParts)
        {
            return (TBody) Activator.CreateInstance(typeof(TBody), BodyPosition, allParts, Configuration.ContentBufferSize);
        }
    }
}