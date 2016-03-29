namespace DataMigratorTest.Container.Base
{
	using System.IO;
	using Configuration;
	using DataMigrator.Container.Base;
	using DataMigrator.Container.Base.Header;
	using DataMigrator.FileSystem;
	using NLog;
	using NLog.Config;
	using NUnit.Framework;
	using Validation.Base;

	public abstract class ContainerTestBase<TContainer, TContentHeader, TFsInfo>
		where TContainer : MigrationContainerInfo<TContainer, TContentHeader, TFsInfo>
		where TContentHeader : class, IFilesystemHeader<TFsInfo>
		where TFsInfo : FileSystemInfo
	{
		protected readonly IContainerValidator<TContainer, TContentHeader, TFsInfo> Validator;

		protected TFsInfo Imported { get; set; }
		protected TContainer MainPart { get; set; }

		protected ContainerTestBase(IContainerValidator<TContainer, TContentHeader, TFsInfo> validator)
		{
			Validator = validator;

			LogManager.Configuration =
				new XmlLoggingConfiguration(Path.Combine(TestConfiguration.Instance.UnitTestDir.FullName, "Nlog.xml"));

			if (!TestConfiguration.Instance.ImportTarget.Exists) TestConfiguration.Instance.ImportTarget.Create();
			if (!TestConfiguration.Instance.ExportTarget.Exists) TestConfiguration.Instance.ExportTarget.Create();
		}

		[TestFixtureTearDown]
		public void ClassCleanup()
		{
			TestConfiguration.Instance.ImportTarget.Refresh();
			foreach (var junction in JunctionPoint.FindJunctions(TestConfiguration.Instance.ImportTarget, SearchOption.AllDirectories))
			{
				junction.Delete();
			}
			TestConfiguration.Instance.ImportTarget.Refresh();
			if (TestConfiguration.Instance.ImportTarget.Exists)
			{
				foreach (var dirInfo in TestConfiguration.Instance.ImportTarget.GetDirectories("*", SearchOption.TopDirectoryOnly))
				{
					dirInfo.Delete(true);
				}
				TestConfiguration.Instance.ImportTarget.Refresh();
				foreach (var fileInfo in TestConfiguration.Instance.ImportTarget.GetFiles("*", SearchOption.TopDirectoryOnly))
				{
					fileInfo.Delete();
				}
			}

			TestConfiguration.Instance.ExportTarget.Refresh();
			if (TestConfiguration.Instance.ExportTarget.Exists)
			{
				foreach (var dirInfo in TestConfiguration.Instance.ExportTarget.GetDirectories("*", SearchOption.TopDirectoryOnly))
				{
					dirInfo.Delete(true);
				}
				TestConfiguration.Instance.ExportTarget.Refresh();
				foreach (var fileInfo in TestConfiguration.Instance.ExportTarget.GetFiles("*", SearchOption.TopDirectoryOnly))
				{
					fileInfo.Delete();
				}
			}
		}

		public abstract void ContentHeaderTest();
		public abstract void ImportTest();
		public abstract void MigrationContainerTest();
		public abstract void StartHeaderTest();
	}
}
