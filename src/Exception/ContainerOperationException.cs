namespace Pawod.MigrationContainer.Exception
{
    public class ContainerOperationException : System.Exception
    {
        public ContainerOperationException(string message, System.Exception ex) : base(message, ex)
        {
        }
    }
}