using FluentAssertions;
using Pawod.MigrationContainer.Container;
using Pawod.MigrationContainer.Filesystem.Base;
using Pawod.MigrationContainer.Filesystem.NTFS;

namespace Pawod.MigrationContainer.Test.Validation
{
    public class NtfsFileContainerValidator : NtfsContainerValidator<NtfsFileContainer, NtfsFile>
    {
        public override void ValidateImport(IFile imported, NtfsFile source)
        {
            ValidateFile((INtfsFile) imported, source);
        }

        public override void ValidatePartedContentHeader(NtfsFileContainer container, NtfsFile source)
        {
            ValidateContentHeader(container, source);
            foreach (var partialContainer in GetRelatedParts(container)) { partialContainer.ContentHeader.Should().NotBeNull(); }
        }
    }
}