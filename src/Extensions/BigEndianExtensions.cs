using System;
using System.Net;

namespace Pawod.MigrationContainer.Extensions
{
    public static class BigEndianExtensions
    {
        public static void FromBigEndian(this byte[] bytes)
        {
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
        }

        public static short FromBigEndian(this short value)
        {
            return IPAddress.NetworkToHostOrder(value);
        }

        public static int FromBigEndian(this int value)
        {
            return IPAddress.NetworkToHostOrder(value);
        }

        public static long FromBigEndian(this long value)
        {
            return IPAddress.NetworkToHostOrder(value);
        }

        public static short ToBigEndian(this short value)
        {
            return IPAddress.HostToNetworkOrder(value);
        }

        public static void ToBigEndian(this byte[] bytes)
        {
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
        }

        public static int ToBigEndian(this int value)
        {
            return IPAddress.HostToNetworkOrder(value);
        }

        public static long ToBigEndian(this long value)
        {
            return IPAddress.HostToNetworkOrder(value);
        }
    }
}