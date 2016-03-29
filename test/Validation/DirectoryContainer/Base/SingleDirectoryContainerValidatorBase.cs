namespace DataMigratorTest.Validation.DirectoryContainer.Base
{
    using System.IO;
    using DataMigrator.Container.Base.Header;
    using DataMigrator.Container.DirectoryContainer;
    using DataMigrator.Container.DirectoryContainer.Base;

	public abstract class SingleDirectoryContainerValidatorBase<TContainer, TDirectoryHeader, TFileHeader> :
        DirectoryContainerValidatorBase<TContainer, TDirectoryHeader, TFileHeader>
        where TContainer : DirectoryContainerInfoBase<TContainer, TDirectoryHeader, TFileHeader>
		where TDirectoryHeader : class, IDirectoryHeaderBase<TDirectoryHeader, TFileHeader>
		where TFileHeader : class, IFilesystemHeader<FileInfo>
    {
    }
}
