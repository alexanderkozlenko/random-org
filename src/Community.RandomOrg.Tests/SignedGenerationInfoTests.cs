using System;
using Community.RandomOrg.Data;
using Xunit;

namespace Community.RandomOrg.Tests
{
    public sealed class SignedGenerationInfoTests
    {
        [Fact]
        public void ConstructorWhenRandomIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => new SignedGenerationInfo<SignedIntegersRandom, int>(null, 0, 0, 0, new byte[32]));
        }

        [Fact]
        public void ConstructorWhenSignatureIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => new SignedGenerationInfo<SignedIntegersRandom, int>(new SignedIntegersRandom(), 0, 0, 0, null));
        }
    }
}