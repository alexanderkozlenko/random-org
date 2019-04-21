using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Anemonis.RandomOrg.UnitTests.Resources;

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
        public async Task GenerateSignedIntegersWithInvalidParameter(int count, int minimum, int maximum)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_int_req.json"));

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpInvoker()))
            {
                await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() =>
                    client.GenerateSignedIntegersAsync(count, minimum, maximum, false));
            }
        }

        [TestMethod]
        public async Task GenerateSignedIntegersWithInvalidUserData()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_int_req.json"));
            var userData = CreateTestString(1001);

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpInvoker()))
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                    client.GenerateSignedIntegersAsync(1, 1, 1, false, userData));
            }
        }

        [TestMethod]
        public async Task GenerateSignedIntegers()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_int_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_int_res.json"));

            var joparams = joreq["params"];
            var jorandom = jores["result"]["random"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpInvoker(joreq, jores)))
            {
                var result = await client.GenerateSignedIntegersAsync(
                    joparams["n"].ToObject<int>(),
                    joparams["min"].ToObject<int>(),
                    joparams["max"].ToObject<int>(),
                    joparams["replacement"].ToObject<bool>(),
                    joparams["userData"].ToObject<string>());

                VerifyResult(result, jores);

                Assert.AreEqual(jorandom["n"].ToObject<int>(), result.Random.Data.Count);

                CollectionAssert.AreEqual(jorandom["data"].ToObject<int[]>(), result.Random.Data?.ToArray());

                Assert.AreEqual(jorandom["min"].ToObject<int>(), result.Random.Parameters.Minimum);
                Assert.AreEqual(jorandom["max"].ToObject<int>(), result.Random.Parameters.Maximum);
                Assert.AreEqual(jorandom["replacement"].ToObject<bool>(), result.Random.Parameters.Replacement);
                Assert.AreEqual(jorandom["userData"].ToObject<string>(), result.Random.UserData);
            }
        }

        [DataTestMethod]
        [DataRow(00000, +0000000000, +0000000005)]
        [DataRow(10001, +0000000000, +0000000005)]
        [DataRow(00001, -1000000001, +0000000005)]
        [DataRow(00001, +1000000001, +0000000005)]
        [DataRow(00001, +0000000000, -1000000001)]
        [DataRow(00001, +0000000000, +1000000001)]
        public async Task GenerateSignedIntegerSequencesWithInvalidParameter(int count, int minimum, int maximum)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_seq_req.json"));

            var counts = new[] { count };
            var minimums = new[] { minimum };
            var maximums = new[] { maximum };
            var replacements = new[] { false };

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpInvoker()))
            {
                await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() =>
                    client.GenerateSignedIntegerSequencesAsync(counts, minimums, maximums, replacements));
            }
        }

        [TestMethod]
        public async Task GenerateSignedIntegerSequencesWithInvalidUserData()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_seq_req.json"));

            var counts = new[] { 1 };
            var minimums = new[] { 1 };
            var maximums = new[] { 1 };
            var replacements = new[] { false };

            var userData = CreateTestString(1001);

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpInvoker()))
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                    client.GenerateSignedIntegerSequencesAsync(counts, minimums, maximums, replacements, userData));
            }
        }

        [DataTestMethod]
        [DataRow(0000)]
        [DataRow(1001)]
        public async Task GenerateSignedIntegerSequencesWithInvalidCount(int sequencesCount)
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
                    client.GenerateSignedIntegerSequencesAsync(counts, minimums, maximums, replacements));
            }
        }

        [TestMethod]
        public async Task GenerateSignedIntegerSequences()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_seq_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_seq_res.json"));

            var joparams = joreq["params"];
            var jorandom = jores["result"]["random"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpInvoker(joreq, jores)))
            {
                var result = await client.GenerateSignedIntegerSequencesAsync(
                    joparams["length"].ToObject<int[]>(),
                    joparams["min"].ToObject<int[]>(),
                    joparams["max"].ToObject<int[]>(),
                    joparams["replacement"].ToObject<bool[]>(),
                    joparams["userData"].ToObject<string>());

                VerifyResult(result, jores);

                Assert.AreEqual(jorandom["n"].ToObject<int>(), result.Random.Data.Count);

                CollectionAssert.AreEqual(jorandom["length"].ToObject<int[]>(), result.Random.Data.Select(s => s.Count).ToArray());

                var jodata = (JArray)jorandom["data"];

                for (var i = 0; i < result.Random.Data.Count; i++)
                {
                    CollectionAssert.AreEqual(jodata[i].ToObject<int[]>(), result.Random.Data[i]?.ToArray());
                }

                CollectionAssert.AreEqual(jorandom["min"].ToObject<int[]>(), result.Random.Parameters.Minimums?.ToArray());
                CollectionAssert.AreEqual(jorandom["max"].ToObject<int[]>(), result.Random.Parameters.Maximums?.ToArray());
                CollectionAssert.AreEqual(jorandom["replacement"].ToObject<bool[]>(), result.Random.Parameters.Replacements?.ToArray());

                Assert.AreEqual(jorandom["userData"].ToObject<string>(), result.Random.UserData);
            }
        }

        [DataTestMethod]
        [DataRow(00000, 02)]
        [DataRow(10001, 02)]
        [DataRow(00001, 00)]
        [DataRow(00001, 21)]
        public async Task GenerateSignedDecimalFractionsWithInvalidParameter(int count, int decimalPlaces)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_dfr_req.json"));

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpInvoker()))
            {
                await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() =>
                    client.GenerateSignedDecimalFractionsAsync(count, decimalPlaces, false));
            }
        }

        [TestMethod]
        public async Task GenerateSignedDecimalFractionsWithInvalidUserData()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_dfr_req.json"));
            var userData = CreateTestString(1001);

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpInvoker()))
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                    client.GenerateSignedDecimalFractionsAsync(1, 1, false, userData));
            }
        }

        [TestMethod]
        public async Task GenerateSignedDecimalFractions()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_dfr_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_dfr_res.json"));

            var joparams = joreq["params"];
            var jorandom = jores["result"]["random"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpInvoker(joreq, jores)))
            {
                var result = await client.GenerateSignedDecimalFractionsAsync(
                    joparams["n"].ToObject<int>(),
                    joparams["decimalPlaces"].ToObject<int>(),
                    joparams["replacement"].ToObject<bool>(),
                    joparams["userData"].ToObject<string>());

                VerifyResult(result, jores);

                Assert.AreEqual(jorandom["n"].ToObject<int>(), result.Random.Data.Count);

                CollectionAssert.AreEqual(jorandom["data"].ToObject<decimal[]>(), result.Random.Data?.ToArray());

                Assert.AreEqual(jorandom["decimalPlaces"].ToObject<int>(), result.Random.Parameters.DecimalPlaces);
                Assert.AreEqual(jorandom["replacement"].ToObject<bool>(), result.Random.Parameters.Replacement);
                Assert.AreEqual(jorandom["userData"].ToObject<string>(), result.Random.UserData);
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
        public async Task GenerateSignedGaussiansWithInvalidParameter(int count, string mean, string standardDeviation, int significantDigits)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_gss_req.json"));

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpInvoker()))
            {
                var meanValue = decimal.Parse(mean, CultureInfo.InvariantCulture);
                var standardDeviationValue = decimal.Parse(standardDeviation, CultureInfo.InvariantCulture);

                await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() =>
                    client.GenerateSignedGaussiansAsync(count, meanValue, standardDeviationValue, significantDigits));
            }
        }

        [TestMethod]
        public async Task GenerateSignedGaussiansWithInvalidUserData()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_gss_req.json"));
            var userData = CreateTestString(1001);

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpInvoker()))
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                    client.GenerateSignedGaussiansAsync(1, 0M, 0M, 2, userData));
            }
        }

        [TestMethod]
        public async Task GenerateSignedGaussians()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_gss_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_gss_res.json"));

            var joparams = joreq["params"];
            var jorandom = jores["result"]["random"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpInvoker(joreq, jores)))
            {
                var result = await client.GenerateSignedGaussiansAsync(
                    joparams["n"].ToObject<int>(),
                    joparams["mean"].ToObject<decimal>(),
                    joparams["standardDeviation"].ToObject<decimal>(),
                    joparams["significantDigits"].ToObject<int>(),
                    joparams["userData"].ToObject<string>());

                VerifyResult(result, jores);

                Assert.AreEqual(jorandom["n"].ToObject<int>(), result.Random.Data.Count);

                CollectionAssert.AreEqual(jorandom["data"].ToObject<decimal[]>(), result.Random.Data?.ToArray());

                Assert.AreEqual(jorandom["mean"].ToObject<decimal>(), result.Random.Parameters.Mean);
                Assert.AreEqual(jorandom["standardDeviation"].ToObject<decimal>(), result.Random.Parameters.StandardDeviation);
                Assert.AreEqual(jorandom["significantDigits"].ToObject<int>(), result.Random.Parameters.SignificantDigits);
                Assert.AreEqual(jorandom["userData"].ToObject<string>(), result.Random.UserData);
            }
        }

        [DataTestMethod]
        [DataRow(00000, 01)]
        [DataRow(10001, 01)]
        [DataRow(00001, 00)]
        [DataRow(00001, 21)]
        public async Task GenerateSignedStringsWithInvalidParameter(int count, int length)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_str_req.json"));
            var characters = CreateTestString(1);

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpInvoker()))
            {
                await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() =>
                    client.GenerateSignedStringsAsync(count, length, characters, false));
            }
        }

        [DataTestMethod]
        [DataRow(00)]
        [DataRow(81)]
        public async Task GenerateSignedStringsWithInvalidCharactersCount(int charactersCount)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_str_req.json"));
            var characters = CreateTestString(charactersCount);

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpInvoker()))
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                    client.GenerateSignedStringsAsync(1, 1, characters, false));
            }
        }

        [TestMethod]
        public async Task GenerateSignedStringsWithCharactersIsNull()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_str_req.json"));

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpInvoker()))
            {
                await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                    client.GenerateSignedStringsAsync(1, 1, null, false));
            }
        }

        [TestMethod]
        public async Task GenerateSignedStringsWithInvalidUserData()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_str_req.json"));
            var characters = CreateTestString(1);
            var userData = CreateTestString(1001);

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpInvoker()))
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                    client.GenerateSignedStringsAsync(1, 1, characters, false, userData));
            }
        }

        [TestMethod]
        public async Task GenerateSignedStrings()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_str_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_str_res.json"));

            var joparams = joreq["params"];
            var jorandom = jores["result"]["random"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpInvoker(joreq, jores)))
            {
                var result = await client.GenerateSignedStringsAsync(
                    joparams["n"].ToObject<int>(),
                    joparams["length"].ToObject<int>(),
                    joparams["characters"].ToObject<string>(),
                    joparams["replacement"].ToObject<bool>(),
                    joparams["userData"].ToObject<string>());

                VerifyResult(result, jores);

                Assert.AreEqual(jorandom["n"].ToObject<int>(), result.Random.Data.Count);

                CollectionAssert.AreEqual(jorandom["data"].ToObject<string[]>(), result.Random.Data?.ToArray());

                Assert.AreEqual(jorandom["length"].ToObject<int>(), result.Random.Parameters.Length);
                Assert.AreEqual(jorandom["characters"].ToObject<string>(), result.Random.Parameters.Characters);
                Assert.AreEqual(jorandom["replacement"].ToObject<bool>(), result.Random.Parameters.Replacement);
                Assert.AreEqual(jorandom["userData"].ToObject<string>(), result.Random.UserData);
            }
        }

        [DataTestMethod]
        [DataRow(0000)]
        [DataRow(1001)]
        public async Task GenerateSignedUuidsWithInvalidParameter(int count)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_uid_req.json"));

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpInvoker()))
            {
                await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() =>
                    client.GenerateSignedUuidsAsync(count));
            }
        }

        [TestMethod]
        public async Task GenerateSignedUuidsWithInvalidUserData()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_uid_req.json"));
            var userData = CreateTestString(1001);

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpInvoker()))
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                    client.GenerateSignedUuidsAsync(1, userData));
            }
        }

        [TestMethod]
        public async Task GenerateSignedUuids()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_uid_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_uid_res.json"));

            var joparams = joreq["params"];
            var jorandom = jores["result"]["random"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpInvoker(joreq, jores)))
            {
                var result = await client.GenerateSignedUuidsAsync(
                    joparams["n"].ToObject<int>(),
                    joparams["userData"].ToObject<string>());

                VerifyResult(result, jores);

                Assert.AreEqual(jorandom["n"].ToObject<int>(), result.Random.Data.Count);

                CollectionAssert.AreEqual(jorandom["data"].ToObject<Guid[]>(), result.Random.Data?.ToArray());

                Assert.AreEqual(jorandom["userData"].ToObject<string>(), result.Random.UserData);
            }
        }

        [DataTestMethod]
        [DataRow(000, 000001)]
        [DataRow(101, 000001)]
        [DataRow(001, 000000)]
        [DataRow(001, 131073)]
        [DataRow(002, 131072)]
        public async Task GenerateSignedBlobsWithInvalidParameter(int count, int size)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_blb_req.json"));

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpInvoker()))
            {
                await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() =>
                    client.GenerateSignedBlobsAsync(count, size));
            }
        }

        [TestMethod]
        public async Task GenerateSignedBlobsWithInvalidUserData()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_blb_req.json"));
            var userData = CreateTestString(1001);

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpInvoker()))
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                    client.GenerateSignedBlobsAsync(1, 1, userData));
            }
        }

        [TestMethod]
        public async Task GenerateSignedBlobs()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_blb_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_blb_res.json"));

            var joparams = joreq["params"];
            var jorandom = jores["result"]["random"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpInvoker(joreq, jores)))
            {
                var result = await client.GenerateSignedBlobsAsync(
                    joparams["n"].ToObject<int>(),
                    joparams["size"].ToObject<int>() / 8,
                    joparams["userData"].ToObject<string>());

                VerifyResult(result, jores);

                Assert.AreEqual(jorandom["n"].ToObject<int>(), result.Random.Data.Count);

                var jodata = (JArray)jorandom["data"];

                for (var i = 0; i < jodata.Count; i++)
                {
                    CollectionAssert.AreEqual(Convert.FromBase64String(jodata[i].ToObject<string>()), result.Random.Data[i]);
                }

                Assert.AreEqual(jorandom["size"].ToObject<int>(), result.Random.Parameters.Size * 8);
                Assert.AreEqual(jorandom["userData"].ToObject<string>(), result.Random.UserData);
            }
        }
    }
}