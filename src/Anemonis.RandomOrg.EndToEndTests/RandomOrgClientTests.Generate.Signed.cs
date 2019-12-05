using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.RandomOrg.EndToEndTests
{
    public partial class RandomOrgClientTests
    {
        [TestMethod]
        public async Task GenerateSignedBlobsAsync()
        {
            using var client = new RandomOrgClient(GetServiceApiKey());

            var result = await client.GenerateSignedBlobsAsync(2, 2).ConfigureAwait(false);

            AssertSignedRandomResult(result);

            Assert.AreEqual(2, result.Random.Data.Count);
            Assert.IsNotNull(result.Random.Data[0]);
            Assert.IsNotNull(result.Random.Data[1]);
            Assert.AreEqual(2, result.Random.Data[0].Length);
            Assert.AreEqual(2, result.Random.Data[1].Length);

            TraceSignedRandomResult(result);

            Trace.WriteLine($"result.random.data.0: {BitConverter.ToString(result.Random.Data[0])}");
            Trace.WriteLine($"result.random.data.1: {BitConverter.ToString(result.Random.Data[1])}");
        }

        [TestMethod]
        public async Task GenerateSignedDecimalFractionsAsync()
        {
            using var client = new RandomOrgClient(GetServiceApiKey());

            var result = await client.GenerateSignedDecimalFractionsAsync(2, 2, false).ConfigureAwait(false);

            AssertSignedRandomResult(result);

            Assert.AreEqual(2, result.Random.Data.Count);

            TraceSignedRandomResult(result);

            Trace.WriteLine($"result.random.data.0: {result.Random.Data[0]:N2}");
            Trace.WriteLine($"result.random.data.1: {result.Random.Data[1]:N2}");
        }

        [TestMethod]
        public async Task GenerateSignedGaussiansAsync()
        {
            using var client = new RandomOrgClient(GetServiceApiKey());

            var result = await client.GenerateSignedGaussiansAsync(2, 0.0m, 1.0m, 2).ConfigureAwait(false);

            AssertSignedRandomResult(result);

            Assert.AreEqual(2, result.Random.Data.Count);

            TraceSignedRandomResult(result);

            Trace.WriteLine($"result.random.data.0: {result.Random.Data[0]:N2}");
            Trace.WriteLine($"result.random.data.1: {result.Random.Data[1]:N2}");
        }

        [TestMethod]
        public async Task GenerateSignedIntegersAsync()
        {
            using var client = new RandomOrgClient(GetServiceApiKey());

            var result = await client.GenerateSignedIntegersAsync(2, 0, 9, false).ConfigureAwait(false);

            AssertSignedRandomResult(result);

            Assert.AreEqual(2, result.Random.Data.Count);

            TraceSignedRandomResult(result);

            Trace.WriteLine($"result.random.data.0: {result.Random.Data[0]}");
            Trace.WriteLine($"result.random.data.1: {result.Random.Data[1]}");
        }

        [TestMethod]
        public async Task GenerateSignedIntegerSequencesAsync()
        {
            using var client = new RandomOrgClient(GetServiceApiKey());

            var result = await client.GenerateSignedIntegerSequencesAsync(new[] { 2, 2 }, new[] { 0, 0 }, new[] { 9, 9 }, new[] { false, false }).ConfigureAwait(false);

            AssertSignedRandomResult(result);

            Assert.AreEqual(2, result.Random.Data.Count);

            TraceSignedRandomResult(result);

            Trace.WriteLine($"result.random.data.0.0: {result.Random.Data[0][0]}");
            Trace.WriteLine($"result.random.data.0.1: {result.Random.Data[0][1]}");
            Trace.WriteLine($"result.random.data.1.0: {result.Random.Data[1][0]}");
            Trace.WriteLine($"result.random.data.1.1: {result.Random.Data[1][1]}");
        }

        [TestMethod]
        public async Task GenerateSignedStringsAsync()
        {
            using var client = new RandomOrgClient(GetServiceApiKey());

            var result = await client.GenerateSignedStringsAsync(2, 2, "abcdef", false).ConfigureAwait(false);

            AssertSignedRandomResult(result);

            Assert.AreEqual(2, result.Random.Data.Count);
            Assert.IsNotNull(result.Random.Data[0]);
            Assert.IsNotNull(result.Random.Data[1]);

            TraceSignedRandomResult(result);

            Trace.WriteLine($"result.random.data.0: {result.Random.Data[0]}");
            Trace.WriteLine($"result.random.data.1: {result.Random.Data[1]}");
        }

        [TestMethod]
        public async Task GenerateSignedUuidsAsync()
        {
            using var client = new RandomOrgClient(GetServiceApiKey());

            var result = await client.GenerateSignedUuidsAsync(2).ConfigureAwait(false);

            AssertSignedRandomResult(result);

            Assert.AreEqual(2, result.Random.Data.Count);

            TraceSignedRandomResult(result);

            Trace.WriteLine($"result.random.data.0: {result.Random.Data[0]}");
            Trace.WriteLine($"result.random.data.1: {result.Random.Data[1]}");
        }
    }
}
