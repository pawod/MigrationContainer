using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using Alphaleonis.Win32.Filesystem;
using Pawod.MigrationContainer.Container.Enumerator.Filesystem;
using Pawod.MigrationContainer.Filesystem.Base;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Pawod.MigrationContainer.Filesystem.NTFS
{
    public class NtfsDirectory : Directory<NtfsFile, NtfsDirectory>, INtfsDirectory
    {
        private IEnumerable<AlternateDataStreamInfo> _alternateStreams;

        private readonly DirectoryInfo _dirInfo;
        private long? _lenght;

        public override NtfsDirectory Parent => new NtfsDirectory(_dirInfo.Parent);


        public NtfsDirectory(DirectoryInfo dirInfo)
        {
            _dirInfo = dirInfo;
        }

        public NtfsDirectory(string path) : this(new DirectoryInfo(Path.GetLongPath(path)))
        {
        }

        public long AlternateDataLength => AlternateStreams.Sum(ads => ads.Size);

        public IEnumerable<AlternateDataStreamInfo> AlternateStreams
            =>
            _alternateStreams ?? (_alternateStreams = _dirInfo.EnumerateAlternateDataStreams().Where(s => !string.IsNullOrWhiteSpace(s.StreamName)));

        public override Dictionary<string, object> Attributes => new Dictionary<string, object> {{"AttributesFlag", _dirInfo.Attributes}};
        public FileAttributes AttributesFlag => (FileAttributes) Attributes["AttributesFlag"];

        public override DateTime DateCreatedUtc => _dirInfo.CreationTimeUtc;
        public override DateTime DateLastModifiedUtc => _dirInfo.LastWriteTimeUtc;

        public override bool Exists => _dirInfo.Exists;

        public override string Extension
        {
            get { throw new InvalidOperationException("Directories do not have an extension."); }
        }

        public override string FullPath => Path.GetLongPath(_dirInfo.FullName);

        public DirectorySecurity GetAccessControl()
        {
            return _dirInfo.GetAccessControl();
        }

        public override IList<IFile> GetFiles(string pattern, SearchOption searchOption)
        {
            if (!Exists) throw new IOException($"Directory does not exist: '{FullPath}'.");
            if (AttributesFlag.HasFlag(FileAttributes.ReparsePoint)) throw new NotSupportedException("This operation is not available for reparse points.");
            return _dirInfo.GetFiles(pattern, searchOption).Select(f => new NtfsFile(f)).Cast<IFile>().ToList();
        }

        public override IList<IFile> GetFiles()
        {
            if (!Exists) throw new IOException($"Directory does not exist: '{FullPath}'.");
            if (AttributesFlag.HasFlag(FileAttributes.ReparsePoint)) throw new NotSupportedException("This operation is not available for reparse points.");
            return _dirInfo.GetFiles().Select(f => new NtfsFile(f)).Cast<IFile>().ToList();
        }

        public IList<JunctionPoint> GetJunctionPoints(SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (!Exists) throw new IOException($"Directory does not exist: '{FullPath}'.");
            if (AttributesFlag.HasFlag(FileAttributes.ReparsePoint)) throw new NotSupportedException("This operation is not available for reparse points.");
            return JunctionPoint.FindJunctions(_dirInfo, searchOption);
        }

        public string GetJunctionTarget()
        {
            return JunctionPoint.GetTarget(_dirInfo.FullName);
        }

        public long GetLength(IList<string> filter = null, bool includeAlternateStreams = false)
        {
            var sum = 0L;
            var enumerator = GetEnumerator(filter);
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.IsLeaf())
                {
                    sum += enumerator.Current.Leaf.Length;
                    if (includeAlternateStreams) sum += enumerator.Current.Leaf.AlternateDataLength;
                }
                else if (includeAlternateStreams) sum += enumerator.Current.Node.AlternateStreams.Sum(ads => ads.Size);
            }
            return sum;
        }

        public bool IsJunctionPoint => JunctionPoint.Exists(_dirInfo);


        public bool IsReparsePoint => AttributesFlag.HasFlag(FileAttributes.ReparsePoint);

        public override long Length => (long) (_lenght ?? (_lenght = GetLength()));


        public override string Name => _dirInfo.Name;

        public override Stream Open(FileMode mode, FileAccess access)
        {
            throw new NotSupportedException(
                      "Directories do not have streams. If you want to access alternate data streams, use the GetAlternateStreams() method.");
        }

        public FileStream OpenAlternateStream(string streamName, FileAccess fileAccess, FileMode fileMode, FileShare fileShare)
        {
            return File.Open($"{FullPath}:{streamName}:$DATA",
                             fileMode,
                             fileAccess,
                             fileShare,
                             ExtendedFileAttributes.BackupSemantics,
                             PathFormat.LongFullPath);
        }

        public override Stream OpenRead()
        {
            throw new NotSupportedException(
                      "Directories do not have streams. If you want to access alternate data streams, use the GetAlternateStreams() method.");
        }

        public override void Refresh()
        {
            _dirInfo.Refresh();
            _alternateStreams = null;
            _lenght = null;
        }

        FileSecurity INtfsFile.GetAccessControl()
        {
            throw new InvalidOperationException("Use the method, that returns a DirectorySecurity instance instead.");
        }

        public override IList<NtfsDirectory> GetDirectories()
        {
            if (AttributesFlag.HasFlag(FileAttributes.ReparsePoint)) throw new NotSupportedException("This operation is not available for reparse points.");
            return _dirInfo.GetDirectories().Select(d => new NtfsDirectory(d)).ToList();
        }

        public override IFileSystemEnumerator<NtfsDirectory, NtfsFile> GetEnumerator()
        {
            return new FileSystemEnumerator<NtfsTreeNode, NtfsDirectory, NtfsFile>(this);
        }

        public override IFileSystemEnumerator<NtfsDirectory, NtfsFile> GetEnumerator(IList<string> filter)
        {
            return new FileSystemEnumerator<NtfsTreeNode, NtfsDirectory, NtfsFile>(this, filter);
        }

        public override string GetOwner()
        {
            return _dirInfo.GetAccessControl().GetOwner(typeof(SecurityIdentifier)).ToString();
        }
    }
}