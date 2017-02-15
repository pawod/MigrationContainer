namespace Pawod.MigrationContainer.Container.Attribute
{
    /// <summary>
    ///     Holds information about a MigrationContainer format.
    /// </summary>
    public class ContainerMetaDescriptionAttribute : System.Attribute
    {
        /// <summary>
        ///     The file extension used for all container files of this type.
        /// </summary>
        public readonly string FileExtension;

        public ContainerMetaDescriptionAttribute(string fileExtension)
        {
            FileExtension = fileExtension;
        }
    }
}