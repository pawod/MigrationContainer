namespace DataMigratorTest.Validation.FileContainer
{
    using System.IO;
    using Base;
    using DataMigrator.Container.FileContainer;
    using DataMigrator.Container.FileContainer.Header;
    using Validation.Base;

    public class PartedNtfsFileContainerValidator :
        PartedFileContainerValidatorBase<NtfsFileContainerInfo, NtfsFileHeader>
    {
        private readonly NtfsContainerValidator _ntfsValidator;

        public PartedNtfsFileContainerValidator()
        {
            _ntfsValidator = new NtfsContainerValidator();
        }

        public override void ValidateContentHeader(NtfsFileContainerInfo container, FileInfo sourceInfo)
        {
            base.ValidateContentHeader(container, sourceInfo);
            _ntfsValidator.ValiateAttributeFlags(sourceInfo, container.ContentHeader);
        }

        public override void ValidateFile(FileInfo imported, FileInfo sourceFile)
        {
            base.ValidateFile(imported, sourceFile);
            _ntfsValidator.ValidateAlternateStreams(imported, sourceFile);
        }
    }
}
