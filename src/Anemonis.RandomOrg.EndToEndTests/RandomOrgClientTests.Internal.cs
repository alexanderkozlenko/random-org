using System;
using System.Diagnostics;
using System.Reflection;
using Anemonis.RandomOrg.Data;
using Anemonis.Runtime.Configuration;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.RandomOrg.EndToEndTests
{
    public partial class RandomOrgClientTests
    {
        private static readonly AssemblyConfiguration _configuration = AssemblyConfiguration.Load(Assembly.GetExecutingAssembly(), "svcconfig");

        private static string GetServiceApiKey()
        {
            var serviceApiKey = _configuration.GetString("random:apiKey");

            if (serviceApiKey == string.Empty)
            {
                Assert.Inconclusive();
            }

            return serviceApiKey;
        }

        private static void AssertRandomResult<TValue>(RandomResult<TValue> result)
        {
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Random);
            Assert.IsNotNull(result.Random.Data);
            Assert.IsTrue(result.BitsUsed >= 0L);
            Assert.IsTrue(result.BitsLeft >= 0L);
            Assert.IsTrue(result.RequestsLeft >= 0L);
            Assert.IsTrue(result.AdvisoryDelay.Ticks >= 0L);
        }

        private static void AssertSignedRandomResult<TValue, TParameters>(SignedRandomResult<TValue, TParameters> result)
            where TParameters : RandomParameters, new()
        {
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Random);
            Assert.IsNotNull(result.Random.Data);
            Assert.IsTrue(result.BitsUsed >= 0L);
            Assert.IsTrue(result.BitsLeft >= 0L);
            Assert.IsTrue(result.RequestsLeft >= 0L);
            Assert.IsTrue(result.AdvisoryDelay.Ticks >= 0L);

            var signature = result.GetSignature();

            Assert.IsNotNull(signature);
            Assert.AreEqual(512, signature.Length);
        }

        private static void TraceRandomResult<T>(RandomResult<T> result)
        {
            Trace.WriteLine($"result.bits_used: {result.BitsUsed}");
            Trace.WriteLine($"result.bits_left: {result.BitsLeft}");
            Trace.WriteLine($"result.requests_left: {result.RequestsLeft}");
            Trace.WriteLine($"result.advisory_delay: {result.AdvisoryDelay}");
            Trace.WriteLine($"result.random.completion_time: {result.Random.CompletionTime:yyyy.MM.dd-HH:mm:ss.ffff}");
        }

        private static void TraceSignedRandomResult<TValue, TParameters>(SignedRandomResult<TValue, TParameters> result)
            where TParameters : RandomParameters, new()
        {
            var signature = result.GetSignature();

            Trace.WriteLine($"result.signature: {BitConverter.ToString(signature).Replace("-", string.Empty)}");
            Trace.WriteLine($"result.bits_used: {result.BitsUsed}");
            Trace.WriteLine($"result.bits_left: {result.BitsLeft}");
            Trace.WriteLine($"result.requests_left: {result.RequestsLeft}");
            Trace.WriteLine($"result.advisory_delay: {result.AdvisoryDelay}");
            Trace.WriteLine($"result.random.completion_time: {result.Random.CompletionTime:yyyy.MM.dd-HH:mm:ss.ffff}");
        }
    }
}
