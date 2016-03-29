namespace DataMigrator.Exception
{
    using System;

    public class MissingPartException : Exception
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
            return string.Format("Could not find all parts of a MigrationContainer. Required: {0}. Found: {1}.",
                parts,
                found);
        }
    }
}
