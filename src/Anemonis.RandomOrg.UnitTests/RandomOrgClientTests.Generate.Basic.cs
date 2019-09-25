using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Anemonis.Resources;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json.Linq;

namespace Anemonis.RandomOrg.UnitTests
{
    public partial class RandomOrgClientTests
    {
        [DataTestMethod]
        [DataRow(00000, +0000000000, +0000000005)]
        [DataRow(10001, +0000000000, +0000000005)]
        [DataRow(00001, -1000000001, +0000000005)]
        [DataRow(00001, +1000000001, +0000000005)]
        [DataRow(00001, +0000000000, -1000000001)]
        [DataRow(00001, +0000000000, +1000000001)]
        public async Task GenerateIntegersWithInvalidParameter(int count, int minimum, int maximum)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_int_req.json"));

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpInvoker()))
            {
                await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() =>
                    client.GenerateIntegersAsync(count, minimum, maximum, false));
            }
        }

        [TestMethod]
        public async Task GenerateIntegers()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_int_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_int_res.json"));

            var joparams = joreq["params"];
            var jorandom = jores["result"]["random"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpInvoker(joreq, jores)))
            {
                var result = await client.GenerateIntegersAsync(
                    joparams["n"].ToObject<int>(),
                    joparams["min"].ToObject<int>(),
                    joparams["max"].ToObject<int>(),
                    joparams["replacement"].ToObject<bool>());

                VerifyResult(result, jores);

                CollectionAssert.AreEqual(jorandom["data"].ToObject<int[]>(), result.Random.Data?.ToArray());
            }
        }

        [DataTestMethod]
        [DataRow(00000, +0000000000, +0000000005)]
        [DataRow(10001, +0000000000, +0000000005)]
        [DataRow(00001, -1000000001, +0000000005)]
        [DataRow(00001, +1000000001, +0000000005)]
        [DataRow(00001, +0000000000, -1000000001)]
        [DataRow(00001, +0000000000, +1000000001)]
        public async Task GenerateIntegerSequencesWithInvalidParameter(int count, int minimum, int maximum)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_seq_req.json"));

            var counts = new[] { count };
            var minimums = new[] { minimum };
            var maximums = new[] { maximum };
            var replacements = new[] { false };

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpInvoker()))
            {
                await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() =>
                    client.GenerateIntegerSequencesAsync(counts, minimums, maximums, replacements));
            }
        }

        [DataTestMethod]
        [DataRow(0000)]
        [DataRow(1001)]
        public async Task GenerateIntegerSequencesWithInvalidCount(int sequencesCount)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_seq_req.json"));

            var counts = new int[sequencesCount];
            var minimums = new int[sequencesCount];
            var maximums = new int[sequencesCount];
            var replacements = new bool[sequencesCount];

            for (var i = 0; i < sequencesCount; i++)
            {
                counts[i] = 1;
                minimums[i] = 1;
                maximums[i] = 5;
                replacements[i] = false;
            }

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpInvoker()))
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                    client.GenerateIntegerSequencesAsync(counts, minimums, maximums, replacements));
            }
        }

        [TestMethod]
        public async Task GenerateIntegerSequences()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_seq_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_seq_res.json"));

            var joparams = joreq["params"];
            var jorandom = jores["result"]["random"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpInvoker(joreq, jores)))
            {
                var result = await client.GenerateIntegerSequencesAsync(
                    joparams["length"].ToObject<int[]>(),
                    joparams["min"].ToObject<int[]>(),
                    joparams["max"].ToObject<int[]>(),
                    joparams["replacement"].ToObject<bool[]>());

                VerifyResult(result, jores);

                var jodata = (JArray)jorandom["data"];

                for (var i = 0; i < result.Random.Data.Count; i++)
                {
                    CollectionAssert.AreEqual(jodata[i].ToObject<int[]>(), result.Random.Data[i]?.ToArray());
                }
            }
        }

        [DataTestMethod]
        [DataRow(00000, 02)]
        [DataRow(10001, 02)]
        [DataRow(00001, 00)]
        [DataRow(00001, 21)]
        public async Task GenerateDecimalFractionsWithInvalidParameter(int count, int decimalPlaces)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_dfr_req.json"));

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpInvoker()))
            {
                await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() =>
                    client.GenerateDecimalFractionsAsync(count, decimalPlaces, false));
            }
        }

        [TestMethod]
        public async Task GenerateDecimalFractions()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_dfr_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_dfr_res.json"));

            var joparams = joreq["params"];
            var jorandom = jores["result"]["random"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpInvoker(joreq, jores)))
            {
                var result = await client.GenerateDecimalFractionsAsync(
                    joparams["n"].ToObject<int>(),
                    joparams["decimalPlaces"].ToObject<int>(),
                    joparams["replacement"].ToObject<bool>());

                VerifyResult(result, jores);

                CollectionAssert.AreEqual(jorandom["data"].ToObject<decimal[]>(), result.Random.Data?.ToArray());
            }
        }

        [DataTestMethod]
        [DataRow(00000, "+0000000.0", "+0000000.0", 02)]
        [DataRow(10001, "+0000000.0", "+0000000.0", 02)]
        [DataRow(00001, "-1000001.0", "+0000000.0", 02)]
        [DataRow(00001, "+1000001.0", "+0000000.0", 02)]
        [DataRow(00001, "+0000000.0", "-1000001.0", 02)]
        [DataRow(00001, "+0000000.0", "+1000001.0", 02)]
        [DataRow(00001, "+0000000.0", "+0000000.0", 01)]
        [DataRow(00001, "+0000000.0", "+0000000.0", 21)]
        public async Task GenerateGaussiansWithInvalidParameter(int count, string mean, string standardDeviation, int significantDigits)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_gss_req.json"));

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpInvoker()))
            {
                var meanValue = decimal.Parse(mean, CultureInfo.InvariantCulture);
                var standardDeviationValue = decimal.Parse(standardDeviation, CultureInfo.InvariantCulture);

                await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() =>
                    client.GenerateGaussiansAsync(count, meanValue, standardDeviationValue, significantDigits));
            }
        }

        [TestMethod]
        public async Task GenerateGaussians()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_gss_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_gss_res.json"));

            var joparams = joreq["params"];
            var jorandom = jores["result"]["random"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpInvoker(joreq, jores)))
            {
                var result = await client.GenerateGaussiansAsync(
                    joparams["n"].ToObject<int>(),
                    joparams["mean"].ToObject<decimal>(),
                    joparams["standardDeviation"].ToObject<decimal>(),
                    joparams["significantDigits"].ToObject<int>());

                VerifyResult(result, jores);

                CollectionAssert.AreEqual(jorandom["data"].ToObject<decimal[]>(), result.Random.Data?.ToArray());
            }
        }

        [DataTestMethod]
        [DataRow(00000, 01)]
        [DataRow(10001, 01)]
        [DataRow(00001, 00)]
        [DataRow(00001, 21)]
        public async Task GenerateStringsWithInvalidParameter(int count, int length)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_str_req.json"));
            var characters = CreateTestString(1);

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpInvoker()))
            {
                await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() =>
                    client.GenerateStringsAsync(count, length, characters, false));
            }
        }

        [DataTestMethod]
        [DataRow(00)]
        [DataRow(81)]
        public async Task GenerateStringsWithInvalidCharactersCount(int charactersCount)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_str_req.json"));
            var characters = CreateTestString(charactersCount);

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpInvoker()))
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                    client.GenerateStringsAsync(00001, 00001, characters, false));
            }
        }

        [TestMethod]
        public async Task GenerateStringsWithCharactersIsNull()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_str_req.json"));

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpInvoker()))
            {
                await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                    client.GenerateStringsAsync(1, 1, null, false));
            }
        }

        [TestMethod]
        public async Task GenerateStrings()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_str_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_str_res.json"));

            var joparams = joreq["params"];
            var jorandom = jores["result"]["random"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpInvoker(joreq, jores)))
            {
                var result = await client.GenerateStringsAsync(
                    joparams["n"].ToObject<int>(),
                    joparams["length"].ToObject<int>(),
                    joparams["characters"].ToObject<string>(),
                    joparams["replacement"].ToObject<bool>());

                VerifyResult(result, jores);

                CollectionAssert.AreEqual(jorandom["data"].ToObject<string[]>(), result.Random.Data?.ToArray());
            }
        }

        [DataTestMethod]
        [DataRow(0000)]
        [DataRow(1001)]
        public async Task GenerateUuidsWithInvalidParameter(int count)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_uid_req.json"));

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpInvoker()))
            {
                await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() =>
                    client.GenerateUuidsAsync(count));
            }
        }

        [TestMethod]
        public async Task GenerateUuids()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_uid_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_uid_res.json"));

            var joparams = joreq["params"];
            var jorandom = jores["result"]["random"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpInvoker(joreq, jores)))
            {
                var result = await client.GenerateUuidsAsync(
                    joparams["n"].ToObject<int>());

                VerifyResult(result, jores);

                CollectionAssert.AreEqual(jorandom["data"].ToObject<Guid[]>(), result.Random.Data?.ToArray());
            }
        }

        [DataTestMethod]
        [DataRow(000, 000001)]
        [DataRow(101, 000001)]
        [DataRow(001, 000000)]
        [DataRow(001, 131073)]
        [DataRow(002, 131072)]
        public async Task GenerateBlobsWithInvalidParameter(int count, int size)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_blb_req.json"));

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpInvoker()))
            {
                await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() =>
                    client.GenerateBlobsAsync(count, size));
            }
        }

        [TestMethod]
        public async Task GenerateBlobs()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_blb_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_blb_res.json"));

            var joparams = joreq["params"];
            var jorandom = jores["result"]["random"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpInvoker(joreq, jores)))
            {
                var result = await client.GenerateBlobsAsync(
                    joparams["n"].ToObject<int>(),
                    joparams["size"].ToObject<int>() / 8);

                VerifyResult(result, jores);

                var jodata = (JArray)jorandom["data"];

                for (var i = 0; i < jodata.Count; i++)
                {
                    CollectionAssert.AreEqual(Convert.FromBase64String(jodata[i].ToObject<string>()), result.Random.Data[i]?.ToArray());
                }
            }
        }
    }
}
