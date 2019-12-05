using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.RandomOrg.EndToEndTests
{
    public partial class RandomOrgClientTests
    {
        [TestMethod]
        public async Task GenerateBlobsAsync()
        {
            using var client = new RandomOrgClient(GetServiceApiKey());

            var result = await client.GenerateBlobsAsync(2, 2).ConfigureAwait(false);

            AssertRandomResult(result);

            Assert.AreEqual(2, result.Random.Data.Count);
            Assert.IsNotNull(result.Random.Data[0]);
            Assert.IsNotNull(result.Random.Data[1]);
            Assert.AreEqual(2, result.Random.Data[0].Length);
            Assert.AreEqual(2, result.Random.Data[1].Length);

            TraceRandomResult(result);

            Trace.WriteLine($"result.random.data.0: {BitConverter.ToString(result.Random.Data[0])}");
            Trace.WriteLine($"result.random.data.1: {BitConverter.ToString(result.Random.Data[1])}");
        }

        [TestMethod]
        public async Task GenerateDecimalFractionsAsync()
        {
            using var client = new RandomOrgClient(GetServiceApiKey());

            var result = await client.GenerateDecimalFractionsAsync(2, 2, false).ConfigureAwait(false);

            AssertRandomResult(result);

            Assert.AreEqual(2, result.Random.Data.Count);

            TraceRandomResult(result);

            Trace.WriteLine($"result.random.data.0: {result.Random.Data[0]:N2}");
            Trace.WriteLine($"result.random.data.1: {result.Random.Data[1]:N2}");
        }

        [TestMethod]
        public async Task GenerateGaussiansAsync()
        {
            using var client = new RandomOrgClient(GetServiceApiKey());

            var result = await client.GenerateGaussiansAsync(2, 0.0m, 1.0m, 2).ConfigureAwait(false);

            AssertRandomResult(result);

            Assert.AreEqual(2, result.Random.Data.Count);

            TraceRandomResult(result);

            Trace.WriteLine($"result.random.data.0: {result.Random.Data[0]:N2}");
            Trace.WriteLine($"result.random.data.1: {result.Random.Data[1]:N2}");
        }

        [TestMethod]
        public async Task GenerateIntegersAsync()
        {
            using var client = new RandomOrgClient(GetServiceApiKey());

            var result = await client.GenerateIntegersAsync(2, 0, 9, false).ConfigureAwait(false);

            AssertRandomResult(result);

            Assert.AreEqual(2, result.Random.Data.Count);

            TraceRandomResult(result);

            Trace.WriteLine($"result.random.data.0: {result.Random.Data[0]}");
            Trace.WriteLine($"result.random.data.1: {result.Random.Data[1]}");
        }

        [TestMethod]
        public async Task GenerateIntegerSequencesAsync()
        {
            using var client = new RandomOrgClient(GetServiceApiKey());

            var result = await client.GenerateIntegerSequencesAsync(new[] { 2, 2 }, new[] { 0, 0 }, new[] { 9, 9 }, new[] { false, false }).ConfigureAwait(false);

            AssertRandomResult(result);

            Assert.AreEqual(2, result.Random.Data.Count);

            TraceRandomResult(result);

            Trace.WriteLine($"result.random.data.0.0: {result.Random.Data[0][0]}");
            Trace.WriteLine($"result.random.data.0.1: {result.Random.Data[0][1]}");
            Trace.WriteLine($"result.random.data.1.0: {result.Random.Data[1][0]}");
            Trace.WriteLine($"result.random.data.1.1: {result.Random.Data[1][1]}");
        }

        [TestMethod]
        public async Task GenerateStringsAsync()
        {
            using var client = new RandomOrgClient(GetServiceApiKey());

            var result = await client.GenerateStringsAsync(2, 2, "abcdef", false).ConfigureAwait(false);

            AssertRandomResult(result);

            Assert.AreEqual(2, result.Random.Data.Count);
            Assert.IsNotNull(result.Random.Data[0]);
            Assert.IsNotNull(result.Random.Data[1]);

            TraceRandomResult(result);

            Trace.WriteLine($"result.random.data.0: {result.Random.Data[0]}");
            Trace.WriteLine($"result.random.data.1: {result.Random.Data[1]}");
        }

        [TestMethod]
        public async Task GenerateUuidsAsync()
        {
            using var client = new RandomOrgClient(GetServiceApiKey());

            var result = await client.GenerateUuidsAsync(2).ConfigureAwait(false);

            AssertRandomResult(result);

            Assert.AreEqual(2, result.Random.Data.Count);

            TraceRandomResult(result);

            Trace.WriteLine($"result.random.data.0: {result.Random.Data[0]}");
            Trace.WriteLine($"result.random.data.1: {result.Random.Data[1]}");
        }
    }
}
