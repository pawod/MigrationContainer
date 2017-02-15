using System;
using System.Configuration;
using Alphaleonis.Win32.Filesystem;
using NLog;
using NLog.Config;
using Path = System.IO.Path;

namespace Pawod.MigrationContainer.Test.Configuration
{
    public class TestConfiguration
    {
        private static TestConfiguration _instance;
        public readonly DirectoryInfo ExportTarget;
        public readonly DirectoryInfo ImportTarget;
        public readonly DirectoryInfo SourceDirs;
        public readonly DirectoryInfo SourceFiles;
        public readonly DirectoryInfo UnitTestDir;


        public static TestConfiguration Instance => _instance ?? (_instance = new TestConfiguration());

        private TestConfiguration()
        {
            UnitTestDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent;
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

            ImportTarget = string.IsNullOrWhiteSpace(path)
                               ? new DirectoryInfo(Path.Combine(UnitTestDir.FullName, "data", "imported"))
                               : new DirectoryInfo(Path.Combine(path, "imported"));
        }
    }
}