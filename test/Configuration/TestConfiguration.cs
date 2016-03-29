namespace DataMigratorTest.Configuration
{
	using System.Configuration;
	using System.IO;

	public class TestConfiguration : TestConfigurationBase
	{
		public static TestConfiguration Instance
		{
			get { return _instance ?? (_instance = new TestConfiguration()); }
		}

		public readonly DirectoryInfo ImportTarget;
		private static TestConfiguration _instance;

		private TestConfiguration()
		{
			var path = ConfigurationManager.AppSettings["TestData"];
			ImportTarget = string.IsNullOrWhiteSpace(path)
				? new DirectoryInfo(Path.Combine(UnitTestDir.FullName, "data", "imported"))
				: new DirectoryInfo(Path.Combine(path, "imported"));
		}
	}
}
