using System;
using System.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pawod.MigrationContainer.Extensions;

namespace Pawod.MigrationContainer.Test.Other
{
    [TestClass]
    public class StreamExtensionsTest
    {
        #region medthods

        private static readonly Random Random = new Random();

        [TestMethod]
        public void CopyToTest()
        {
            for (var n = 0; n < 1000000; n++)
            {
                var sourceBytes = GetRandomByteArray(512);
                var targetBytes = GetRandomByteArray(512);
                var source = new MemoryStream(sourceBytes);
                var target = new MemoryStream(targetBytes);

                var sourceOffset = Random.Next((int) source.Length);
                var targetOffset = Random.Next((int) target.Length);

                var targetLimit = (int) target.Length - targetOffset;
                var sourceLimit = (int) source.Length - sourceOffset;

                var countLimit = targetLimit < sourceLimit ? targetLimit : sourceLimit;
                var count = Random.Next(countLimit + 1);

                var bufferSize = Random.Next(1, count + 1);

                source.CopyTo(target, sourceOffset, targetOffset, count, bufferSize);

                targetBytes = target.ToArray();
                for (var i = 0; i < count; i++) { targetBytes[targetOffset + i].Should().Be(sourceBytes[sourceOffset + i]); }
            }
        }

        private byte[] GetRandomByteArray(int maxSize)
        {
            var bytes = new byte[Random.Next(1, maxSize)];
            for (var i = 0; i < bytes.Length; i++) { bytes[i] = (byte) Random.Next(maxSize); }
            return bytes;
        }

        #endregion
    }
}