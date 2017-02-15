using System.Collections.Generic;
using System.Linq;
using Pawod.MigrationContainer.Container.Header.NTFS;
using Pawod.MigrationContainer.Filesystem.NTFS;

namespace Pawod.MigrationContainer.Serialization.Partitioning
{
    public class NtfsFileContainerPartitioner : FileContainerPartitioner<NtfsFileHeader, NtfsFile>
    {
        public override IPartitioningScheme GetPartitioningScheme(NtfsFile file,
                                                                  NtfsFileHeader fileHeader,
                                                                  long mainPartBodyLength,
                                                                  long bodyLength,
                                                                  IList<string> filter = null)
        {
            var scheme = base.GetPartitioningScheme(file.FullPath, file.Length, mainPartBodyLength, bodyLength);

            if ((fileHeader.AlternateStreamHeaders != null) && fileHeader.AlternateStreamHeaders.Any())
            {
                var part = scheme.NumberOfParts == 0 ? 0 : scheme.NumberOfParts - 1;
                var bodyOffset = scheme.NumberOfParts > 1 ? scheme.GetPartitionInfo(part).Sum(s => s.Length) : 0;
                var remainingPartitionSpace = (part == 0 ? mainPartBodyLength : bodyLength) - bodyOffset;

                foreach (var alternateStream in file.AlternateStreams)
                {
                    var sourcePosition = 0L;
                    var remainingContentLength = alternateStream.Size;

                    while (remainingContentLength > 0)
                    {
                        var streamInfo = CreatePartitionInfo(alternateStream.FullPath,
                                                             part,
                                                             ref remainingContentLength,
                                                             mainPartBodyLength,
                                                             bodyLength,
                                                             bodyOffset,
                                                             sourcePosition);
                        scheme.AddPartitionInfo(part, streamInfo);

                        sourcePosition += streamInfo.Length;
                        remainingPartitionSpace -= streamInfo.Length;
                        if (remainingPartitionSpace == 0)
                        {
                            part++;
                            bodyOffset = 0;
                            remainingPartitionSpace = part == 0 ? mainPartBodyLength : bodyLength;
                            continue;
                        }
                        bodyOffset += streamInfo.Length;
                    }
                }
            }

            return scheme;
        }
    }
}