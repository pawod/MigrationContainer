using System;
using System.IO;
using System.Text;
using Pawod.MigrationContainer.Container.Header.Base;
using Pawod.MigrationContainer.Filesystem.Base;
using Pawod.MigrationContainer.Serialization.Partitioning;

namespace Pawod.MigrationContainer.Serialization.Parameters
{
    public class SerializationParameters<THeader, TSource, TExport> : ISerializationParameters<THeader, TSource, TExport>
        where THeader : class, IFileHeader
        where TSource : IFile
        where TExport : IFile

    {
        public long AllHeadersLength { get; set; }
        public MemoryStream[] AppHeaderStreams { get; set; }
        public Guid ContainerId { get; set; }
        public THeader ContentHeader { get; set; }
        public MemoryStream ContentHeaderStream { get; set; }
        public string FormatExtension { get; set; }

        public TExport GetTargetFile(int partNumber)
        {
            var extension = GetFileExtension(partNumber, PartitioningScheme.NumberOfParts);
            var path = $"{TargetDir.FullPath}{Path.DirectorySeparatorChar}{Source.Name}{extension}";
            return (TExport) Activator.CreateInstance(typeof(TExport), path);
        }

        public long MaxContainerFileSize { get; set; }
        public IPartitioningScheme PartitioningScheme { get; set; }
        public int RequiredFiles { get; set; }
        public TSource Source { get; set; }
        public IDirectory TargetDir { get; set; }


        private string GetFileExtension(int partNumber, int numberOfFiles)
        {
            var extension = FormatExtension;
            if (partNumber != 0 && numberOfFiles > 1)
            {
                var maxDigits = Math.Ceiling(Math.Log10(numberOfFiles + 1));
                var digits = Math.Ceiling(Math.Log10(partNumber + 1));
                var padding = maxDigits - digits;

                var sb = new StringBuilder();
                for (var i = 0; i < padding; i++) { sb.Append("0"); }

                extension = $"{extension}.part{sb}{partNumber}";
            }
            return extension;
        }
    }
}