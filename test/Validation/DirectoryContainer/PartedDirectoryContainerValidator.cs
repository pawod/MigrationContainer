namespace DataMigratorTest.Validation.DirectoryContainer
{
    using Base;
    using DataMigrator.Container.DirectoryContainer;
    using DataMigrator.Container.DirectoryContainer.Header;
    using DataMigrator.Container.FileContainer.Header;

	public class PartedDirectoryContainerValidator :
        PartedDirectoryContainerValidatorBase<DirectoryContainerInfo, DirectoryHeader, FileHeader>
    {
    }
}
