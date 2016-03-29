namespace DataMigrator.Container.NtfsDirectoryContainer
{
    using System.Collections.Generic;
    using Base.Header;
    using ProtoBuf;

    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class JunctionHeader : ProtoHeader
    {
        public bool IsRelativeTarget { get; private set; }
        public string Name { get; private set; }
        public string Target { get; private set; }
        public IDictionary<string, long> TimeStamps { get; private set; }

        protected JunctionHeader()
        {
        }

        public JunctionHeader(string name, string target, bool isRelativeTarget, IDictionary<string, long> timeStamps)
        {
            Name = name;
            Target = target;
            IsRelativeTarget = isRelativeTarget;
            TimeStamps = timeStamps;
        }
    }
}
