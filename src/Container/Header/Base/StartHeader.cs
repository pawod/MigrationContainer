using System;
using System.Collections.ObjectModel;
using Pawod.MigrationContainer.Exception;
using Pawod.MigrationContainer.Extensions;
using Pawod.MigrationContainer.Filesystem.Base;

namespace Pawod.MigrationContainer.Container.Header.Base
{
    /// <summary>
    ///     The StartHeader contains a minimum set of information, which is required to
    ///     start the deserialization process of a MigrationContainer. Every must start
    ///     MigrationContainer starts with such a header.
    /// </summary>
    public sealed class StartHeader : Header, IStartHeader
    {
        public const int GUID_LENGTH = 128/8;
        public const int INT32_LENGTH = 32/8;
        public const int INT64_LENGTH = 64/8;
        public const int MD5_LENGTH = 128/8;
        public static readonly long Length;
        public static readonly ReadOnlyCollection<byte> MagicNumbers;

        static StartHeader()
        {
            MagicNumbers = Array.AsReadOnly(new Guid("2BEE45B3-AFB3-4118-A044-50B00D9A2D1B").ToByteArray());
            Length = 2*GUID_LENGTH + MD5_LENGTH + 2*INT32_LENGTH + INT64_LENGTH;
        }

        public Guid ContainerId { get; private set; }
        public ReadOnlyCollection<byte> Md5Hash { get; private set; }
        public string Md5String { get; private set; }
        public int PartNumber { get; private set; }
        public int Parts { get; private set; }

        /// <summary>
        ///     Retreives the StartHeader from a serialized MigrationContainer.
        /// </summary>
        /// <param name="containerFile">The serialized migration container.</param>
        /// <returns>The deserialized StartHeader of the specified migration container.</returns>
        public static StartHeader Extract(IFile containerFile)
        {
            if (!containerFile.IsMigrationContainer()) throw new InvalidContainerException();

            using (var fileStream = containerFile.OpenRead())
            {
                var hash = new byte[MD5_LENGTH];
                fileStream.Position = GUID_LENGTH;
                fileStream.Read(hash, 0, hash.Length);
                hash.FromBigEndian();
                var md5String = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();

                var buffer = new byte[GUID_LENGTH];
                fileStream.Read(buffer, 0, buffer.Length);
                buffer.FromBigEndian();
                var containerId = new Guid(buffer);

                buffer = new byte[INT32_LENGTH];
                fileStream.Read(buffer, 0, buffer.Length);
                buffer.FromBigEndian();
                var partNumber = BitConverter.ToInt32(buffer, 0);

                buffer = new byte[INT32_LENGTH];
                fileStream.Read(buffer, 0, buffer.Length);
                buffer.FromBigEndian();
                var numberOfParts = BitConverter.ToInt32(buffer, 0);

                buffer = new byte[INT64_LENGTH];
                fileStream.Read(buffer, 0, buffer.Length);
                buffer.FromBigEndian();
                var nextHeaderLength = BitConverter.ToInt64(buffer, 0);

                return new StartHeader
                       {
                           Md5Hash = Array.AsReadOnly(hash),
                           Md5String = md5String,
                           ContainerId = containerId,
                           PartNumber = partNumber,
                           Parts = numberOfParts,
                           NextHeaderLength = nextHeaderLength
                       };
            }
        }
    }
}