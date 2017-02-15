namespace Pawod.MigrationContainer.Exception
{
    public class MainPartNotFoundException : System.Exception
    {
        public string CurrentPath { get; protected set; }

        public MainPartNotFoundException(string currentPath) : base(CreateMessage(currentPath))
        {
            CurrentPath = currentPath;
        }

        private static string CreateMessage(string currentPath)
        {
            return $"Main part of MigrationContainer could not be found at : '{currentPath}'.";
        }
    }
}