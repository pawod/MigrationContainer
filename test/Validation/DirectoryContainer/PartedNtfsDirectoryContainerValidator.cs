namespace DataMigratorTest.Validation.DirectoryContainer
{
    using System.IO;
    using Base;
    using DataMigrator.Container.FileContainer.Header;
    using DataMigrator.Container.NtfsDirectoryContainer;
    using Validation.Base;

    public class PartedNtfsDirectoryContainerValidator :
        PartedDirectoryContainerValidatorBase<NtfsDirectoryContainerInfo, NtfsDirectoryHeader, NtfsFileHeader>
    {
        private readonly NtfsContainerValidator _ntfsValidator;

        public PartedNtfsDirectoryContainerValidator()
        {
            _ntfsValidator = new NtfsContainerValidator();
        }

        public override void ValidateContentHeader(NtfsDirectoryContainerInfo container, DirectoryInfo sourceInfo)
        {
            base.ValidateContentHeader(container, sourceInfo);
            _ntfsValidator.ValiateAttributeFlags(sourceInfo, container.ContentHeader);
        }

        public override void ValidateDirectory(DirectoryInfo imported, DirectoryInfo sourceDir)
        {
            base.ValidateDirectory(imported, sourceDir);
            _ntfsValidator.ValidateAlternateStreams(imported, sourceDir);
            _ntfsValidator.ValidateJunctions(imported, sourceDir);
        }

        public override void ValidateFile(FileInfo imported, FileInfo sourceFile)
        {
            base.ValidateFile(imported, sourceFile);
            _ntfsValidator.ValidateAlternateStreams(imported, sourceFile);
        }
    }
}
