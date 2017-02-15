using Pawod.MigrationContainer.Container.Header.Base;
using ProtoBuf;

namespace Pawod.MigrationContainer.Container.Header.NTFS
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class JunctionHeader : ProtoHeader
    {
        /// <summary>
        ///     Indicates whether the target path is relative or absolute.
        /// </summary>
        public bool IsRelativeTarget { get; set; }

        /// <summary>
        ///     The junction's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The path to the directory that is targeted.
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        ///     The time UNIX timestamp this file has been created (UTC).
        /// </summary>
        public long TimeCreatedUtc { get; set; }

        /// <summary>
        ///     The time UNIX timestamp this file has last modified (UTC).
        /// </summary>
        public long TimeModifiedUtc { get; set; }

        public JunctionHeader(bool isRelativeTarget, string name, string target, long timeCreatedUtc, long timeModifiedUtc)
        {
            IsRelativeTarget = isRelativeTarget;
            Name = name;
            Target = target;
            TimeCreatedUtc = timeCreatedUtc;
            TimeModifiedUtc = timeModifiedUtc;
        }


        public JunctionHeader()
        {
        }
    }
}