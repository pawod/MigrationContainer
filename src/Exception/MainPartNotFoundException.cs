namespace DataMigrator.Exception
{
    using System;

    public class MainPartNotFoundException : Exception
    {
        public string CurrentPath { get; protected set; }

        public MainPartNotFoundException(string currentPath) : base(CreateMessage(currentPath))
        {
            CurrentPath = currentPath;
        }

        private static string CreateMessage(string currentPath)
        {
            return string.Format("Main part of MigrationContainer could not be found at : '{0}'.", currentPath);
        }
    }
}
