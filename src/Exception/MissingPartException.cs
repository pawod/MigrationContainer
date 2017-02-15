namespace Pawod.MigrationContainer.Exception
{
    public class MissingPartException : System.Exception
    {
        public int Found { get; protected set; }
        public int Parts { get; protected set; }

        public MissingPartException(int parts, int found) : base(CreateMessage(parts, found))
        {
            Parts = parts;
            Found = found;
        }

        private static string CreateMessage(int parts, int found)
        {
            return $"Could not find all parts of a MigrationContainer. Required: {parts}. Found: {found}.";
        }
    }
}