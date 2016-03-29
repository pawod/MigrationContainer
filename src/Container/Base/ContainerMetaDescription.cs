namespace DataMigrator.Container.Base
{
    using System;

    /// <summary>
    ///     Holds information about a MigrationContainer format.
    /// </summary>
    public class ContainerMetaDescription : Attribute
    {
        /// <summary>
        ///     The file extension used for all container files of this type.
        /// </summary>
        public readonly string FileExtension;

        public ContainerMetaDescription(string fileExtension)
        {
            FileExtension = fileExtension;
        }
    }
}
