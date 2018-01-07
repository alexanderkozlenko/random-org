using System;
using Community.RandomOrg.Data;
using Xunit;

namespace Community.RandomOrg.Tests
{
    public sealed class RandomResultObjectTests
    {
        [Fact]
        public void BasicWhenRandomIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new RandomResult<int>(null, 0, 0, 0));
        }

        [Fact]
        public void SignedWhenRandomIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new SignedRandomResult<int, IntegerParameters>(null, 0, 0, 0, new byte[32]));
        }

        [Fact]
        public void SignedWhenSignatureIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new SignedRandomResult<int, IntegerParameters>(new SignedRandom<int, IntegerParameters>(), 0, 0, 0, null));
        }
    }
}