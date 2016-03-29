namespace DataMigrator.Configuration
{
    using System;
    using System.Configuration;
    using ProtoBuf.Meta;

    /// <summary>
    ///     Provides centralized access to protobuf-net's RuntimeTypeModel for
    ///     configuration purposes.
    /// </summary>
    public class ProtoConfiguration
    {
		/// <summary>
        ///     The largest tag used by the framework.
        /// </summary>
        public const int LARGEST_FRAMEWORK_TAG = 11;

        private static ProtoConfiguration _instance;
        private readonly RuntimeTypeModel _model;

        public static ProtoConfiguration Instance
        {
            get { return _instance ?? (_instance = new ProtoConfiguration()); }
        }

        /// <summary>
        ///     The largest tag number used so far.
        /// </summary>
        public int LargestUsedTag { get; private set; }

		/// <summary>
		///     The buffer size to be used when (de-)serializing ProtoHeaders.
		/// </summary>
		public int BufferSize { get; set; }

        private ProtoConfiguration()
        {
            _model = TypeModel.Create();
            LargestUsedTag = LARGEST_FRAMEWORK_TAG;
	        BufferSize = 4096;
        }

        /// <summary>
        ///     Adds a subtype to the existing class hierarchy of protobuf serializable
        ///     headers.
        /// </summary>
        /// <param name="basetype">The base type of the extended type.</param>
        /// <param name="derivedType">The type inheriting from the base type.</param>
        /// <param name="protoTag">The proto tag to be used for the derived type.</param>
        public void AddSubType(Type basetype, Type derivedType, int protoTag)
        {
            _model[basetype].AddSubType(protoTag, derivedType);
            if (LargestUsedTag < protoTag) LargestUsedTag = protoTag;
        }
    }
}
