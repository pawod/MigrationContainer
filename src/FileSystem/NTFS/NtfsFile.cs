using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using Alphaleonis.Win32.Filesystem;
using Pawod.MigrationContainer.Filesystem.Base;
using File = Alphaleonis.Win32.Filesystem.File;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Pawod.MigrationContainer.Filesystem.NTFS
{
    public class NtfsFile : File<NtfsFile, NtfsDirectory>, INtfsFile
    {
        private IEnumerable<AlternateDataStreamInfo> _alternateStreams;
        private readonly FileInfo _fileInfo;
        public override NtfsDirectory Parent => new NtfsDirectory(_fileInfo.Directory);

        public NtfsFile(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
        }

        public NtfsFile(string filePath) : this(new FileInfo(Path.GetLongPath(filePath)))
        {
        }

        public long AlternateDataLength => AlternateStreams.Sum(ads => ads.Size);

        public IEnumerable<AlternateDataStreamInfo> AlternateStreams
            =>
            _alternateStreams ?? (_alternateStreams = _fileInfo.EnumerateAlternateDataStreams().Where(s => !string.IsNullOrWhiteSpace(s.StreamName)));

        public override Dictionary<string, object> Attributes => new Dictionary<string, object> {{"AttributesFlag", _fileInfo.Attributes}};
        public FileAttributes AttributesFlag => (FileAttributes) Attributes["AttributesFlag"];

        public override DateTime DateCreatedUtc => _fileInfo.CreationTimeUtc;
        public override DateTime DateLastModifiedUtc => _fileInfo.LastWriteTimeUtc;
        public override bool Exists => _fileInfo.Exists;
        public override string Extension => _fileInfo.Extension;

        public override string FullPath => Path.GetLongPath(_fileInfo.FullName);

        public FileSecurity GetAccessControl()
        {
            return _fileInfo.GetAccessControl();
        }

        public override long Length => _fileInfo.Length;

        public override string Name => _fileInfo.Name;

        public override Stream Open(FileMode mode, FileAccess access)
        {
            return _fileInfo.Open(mode, access);
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
            return _fileInfo.OpenRead();
        }

        public override void Refresh()
        {
            _fileInfo.Refresh();
            _alternateStreams = null;
        }

        public override string GetOwner()
        {
            return _fileInfo.GetAccessControl().GetOwner(typeof(SecurityIdentifier)).ToString();
        }
    }
}