namespace DataMigrator.Exception
{
    using System;

    public class ContainerOperationException : Exception
    {
        public ContainerOperationException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
