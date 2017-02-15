using System;
using ProtoBuf.Meta;

namespace Pawod.MigrationContainer.Configuration
{
    /// <summary>
    ///     Provides centralized access to protobuf-net's RuntimeTypeModel for configuration purposes.
    /// </summary>
    public class ProtoConfiguration
    {
        /// <summary>
        ///     The largest tag used by the MigrationContainer framework.
        /// </summary>
        public const int LARGEST_DEFAULT_TAG = 8;

        private static ProtoConfiguration _instance;

        public static ProtoConfiguration Instance => _instance ?? (_instance = new ProtoConfiguration());

        /// <summary>
        ///     The largest proto-tag number used so far.
        /// </summary>
        public int LargestTagInUse { get; private set; }

        private ProtoConfiguration()
        {
            LargestTagInUse = LARGEST_DEFAULT_TAG;
        }

        /// <summary>
        ///     Adds a subtype to the existing class hierarchy of protobuf serializable headers. NOTE: You should use this method
        ///     in a way, that reliably allows you to reproduce the prototag for a specific subtype. E.g. in a singleton
        ///     configuration class, that is always initialized first in your code.
        /// </summary>
        /// <param name="basetype">The base type of the extended type.</param>
        /// <param name="derivedType">The type inheriting from the base type.</param>
        /// <param name="protoTag">The proto tag to be used for the derived type.</param>
        public void AddSubType(Type basetype, Type derivedType, int protoTag)
        {
            if (protoTag < LargestTagInUse + 1)
                throw new System.Exception(
                          "Illegal proto-tag. The proto-tag for this protobuf message must be greater than the smallest one already in use.");

            RuntimeTypeModel.Default[basetype].AddSubType(protoTag, derivedType);
            if (protoTag > LargestTagInUse) LargestTagInUse = protoTag;
        }

        /// <summary>
        ///     Does the same as AddSubType(), but for types that inherit from generic base types.
        /// </summary>
        /// <param name="genericBaseType">The generic base type of the extended type (with non-abstract type parameters).</param>
        /// <param name="baseBaseType">The (non-generic) type the generic base type derives from.</param>
        /// <param name="derivedType">The type inheriting from the base type.</param>
        /// <param name="protoTag">The proto tag to be used for the derived type.</param>
        public void AddSubTypeForGenericBaseType(Type genericBaseType, Type baseBaseType, Type derivedType, int protoTag)
        {
            if (protoTag < LargestTagInUse + 1)
                throw new System.Exception(
                          "Illegal proto-tag. The proto-tag for this protobuf message must be greater than the smallest one already in use.");

            // add the generic base type with specific type arguments to the class hierarchy
            RuntimeTypeModel.Default[baseBaseType].AddSubType(protoTag++, genericBaseType);
            // now link the derived type to the generic base type
            RuntimeTypeModel.Default.Add(genericBaseType, true);
            RuntimeTypeModel.Default[genericBaseType].AddSubType(protoTag, derivedType);
            if (protoTag > LargestTagInUse) LargestTagInUse = protoTag;
        }
    }
}