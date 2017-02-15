namespace Pawod.MigrationContainer.Container.Header.Base
{
    public interface IHeader
    {
        /// <summary>
        ///     The length of the subsequent header in bytes.
        /// </summary>
        long NextHeaderLength { get; }

        /// <summary>
        ///     Determines wether this header is the last one, before the content section
        ///     starts.
        /// </summary>
        bool IsLastHeader();
    }
}