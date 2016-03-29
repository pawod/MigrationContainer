namespace DataMigratorTest.Configuration
{
	using System;
	using System.Configuration;
	using System.IO;
	using NLog;
	using NLog.Config;

	public abstract class TestConfigurationBase
	{
		public readonly DirectoryInfo ExportTarget;
		public readonly DirectoryInfo SourceDirs;
		public readonly DirectoryInfo SourceFiles;
		public readonly DirectoryInfo UnitTestDir;

		protected TestConfigurationBase()
		{
			UnitTestDir = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent;
			LogManager.Configuration = new XmlLoggingConfiguration(Path.Combine(UnitTestDir.FullName, "Nlog.xml"));

			var path = ConfigurationManager.AppSettings["TestData"];
			if (string.IsNullOrWhiteSpace(path))
			{
				SourceDirs = new DirectoryInfo(Path.Combine(UnitTestDir.FullName, "data", "dirs"));
				SourceFiles = new DirectoryInfo(Path.Combine(UnitTestDir.FullName, "data", "files"));
				ExportTarget = new DirectoryInfo(Path.Combine(UnitTestDir.FullName, "data", "exported"));
			}
			else
			{
				SourceDirs = new DirectoryInfo(Path.Combine(path, "dirs"));
				SourceFiles = new DirectoryInfo(Path.Combine(path, "files"));

				ExportTarget = new DirectoryInfo(Path.Combine(path, "exported"));
			}
		}
	}
}
