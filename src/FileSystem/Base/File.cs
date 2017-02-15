using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Pawod.MigrationContainer.Container.Header.Base;
using Pawod.MigrationContainer.Extensions;

namespace Pawod.MigrationContainer.Filesystem.Base
{
    /// <summary>
    ///     An abstraction of a file on a file system.
    /// </summary>
    public abstract class File<TFile, TDirectory> : IFile
        where TFile : File<TFile, TDirectory>
        where TDirectory : Directory<TFile, TDirectory>
    {
        public abstract bool Exists { get; }
        public abstract TDirectory Parent { get; }
        public abstract Dictionary<string, object> Attributes { get; }
        public abstract DateTime DateCreatedUtc { get; }
        public abstract DateTime DateLastModifiedUtc { get; }
        public abstract string Extension { get; }
        public abstract string FullPath { get; }

        public virtual bool IsMigrationContainer()
        {
            using (var fileStream = OpenRead())
            {
                var magicNumbers = new byte[StartHeader.MagicNumbers.Count];
                fileStream.Read(magicNumbers, 0, StartHeader.MagicNumbers.Count);
                magicNumbers.FromBigEndian();
                return magicNumbers.SequenceEqual(StartHeader.MagicNumbers);
            }
        }

        public abstract long Length { get; }
        public abstract string Name { get; }
        public abstract Stream Open(FileMode mode, FileAccess access);
        public abstract Stream OpenRead();
        public abstract void Refresh();
        bool IFile.Exists => Exists;
        IDirectory IFile.Parent => Parent;
        public abstract string GetOwner();
    }
}