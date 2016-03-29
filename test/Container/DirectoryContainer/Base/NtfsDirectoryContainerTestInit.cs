namespace DataMigratorTest.Container.DirectoryContainer.Base
{
	using System.IO;
	using Configuration;
	using DataMigrator.Adapters.NTFS.Directory;
	using DataMigrator.Container.FileContainer.Header;
	using DataMigrator.Container.NtfsDirectoryContainer;
	using DataMigrator.Factory.Container.Directory;
	using DataMigrator.Factory.Parameters;
	using Validation.Base;

	public abstract class NtfsDirectoryContainerTestInit :
        DirectoryContainerTestBase<NtfsDirectoryContainerInfo, NtfsDirectoryHeader, NtfsFileHeader>
    {
        protected NtfsDirectoryContainerTestInit(string dirName,
                                                 IContainerValidator
                                                     <NtfsDirectoryContainerInfo, NtfsDirectoryHeader, DirectoryInfo>
                                                     validator) : base(dirName, validator)
        {
            var containerFactory = new NtfsDirectoryContainerFactory();
            var parameterFactory = new NtfsDirectorySerializationParametersFactory();
            var parameters = parameterFactory.Create(SourceDir, TestConfiguration.Instance.ExportTarget);

            MainPart = containerFactory.Create(parameters);
            Imported =
				new DirectoryInfo(new NtfsDirectoryAdapter().Import(MainPart, TestConfiguration.Instance.ImportTarget.FullName));
        }
    }
}
