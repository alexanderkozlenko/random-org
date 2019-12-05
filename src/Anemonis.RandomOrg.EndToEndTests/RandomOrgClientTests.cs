using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.RandomOrg.EndToEndTests
{
    [TestClass]
    public sealed partial class RandomOrgClientTests
    {
        [TestMethod]
        public async Task GetUsageAsync()
        {
            using var client = new RandomOrgClient(GetServiceApiKey());

            var result = await client.GetUsageAsync().ConfigureAwait(false);

            Assert.IsNotNull(result);

            Trace.WriteLine($"result.status: {result.Status}");
            Trace.WriteLine($"result.bits_left: {result.BitsLeft}");
            Trace.WriteLine($"result.requests_left: {result.RequestsLeft}");
        }

        [TestMethod]
        public async Task VerifySignatureAsync()
        {
            using var client = new RandomOrgClient(GetServiceApiKey());

            var result = await client.GenerateSignedIntegersAsync(1, 0, 1, false).ConfigureAwait(false);
            var authenticity = await client.VerifySignatureAsync(result.Random, result.GetSignature()).ConfigureAwait(false);

            Assert.IsTrue(authenticity);
        }
    }
}
