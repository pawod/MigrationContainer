namespace DataMigratorTest.Validation.Base
{
    using System.IO;
    using DataMigrator.Container.Base;
    using DataMigrator.Container.Base.Header;

	public interface IContainerValidator<in TContainer, in TContentHeader, in TFsInfo>
        where TContainer : MigrationContainerInfo<TContainer, TContentHeader, TFsInfo>
		where TContentHeader : class, IFilesystemHeader<TFsInfo>
        where TFsInfo : FileSystemInfo
    {
        void ValidateContentHeader(TContainer container, TFsInfo sourceInfo);
        void ValidateFile(FileInfo imported, FileInfo sourceFile);
        void ValidateImport(TFsInfo imported, TFsInfo sourceInfo);
        void ValidateMigrationContainer(TContainer container, TFsInfo sourceInfo);
        void ValidateStartHeader(TContainer container, TFsInfo sourceInfo);
    }
}
