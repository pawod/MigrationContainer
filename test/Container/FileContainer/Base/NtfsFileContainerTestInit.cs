namespace DataMigratorTest.Container.FileContainer.Base
{
	using System.IO;
	using Configuration;
	using DataMigrator.Adapters.NTFS.File;
	using DataMigrator.Container.FileContainer;
	using DataMigrator.Container.FileContainer.Header;
	using DataMigrator.Factory.Container.File;
	using DataMigrator.Factory.Parameters;
	using Validation.Base;

	public abstract class NtfsFileContainerTestInit : FileContainerTestBase<NtfsFileContainerInfo, NtfsFileHeader>
    {
        protected NtfsFileContainerTestInit(string fileName,
                                            IContainerValidator<NtfsFileContainerInfo, NtfsFileHeader, FileInfo>
                                                validator) : base(fileName, validator)
        {
            var containerFactory = new NtfsFileContainerFactory();
            var parameterFactory = new NtfsFileSerializationParametersFactory();
			var parameters = parameterFactory.Create(SourceFile, TestConfiguration.Instance.ExportTarget);

            MainPart = containerFactory.Create(parameters);
			Imported = new FileInfo(new NtfsFileAdapter().Import(MainPart, TestConfiguration.Instance.ImportTarget.FullName));
        }
    }
}
