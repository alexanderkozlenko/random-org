using System;
using Community.RandomOrg.Data;
using Xunit;

namespace Community.RandomOrg.Tests
{
    public sealed class SimpleGenerationInfoTests
    {
        [Fact]
        public void ConstructorWhenRandomIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => new SimpleGenerationInfo<int>(null, 0, 0, 0));
        }
    }
}