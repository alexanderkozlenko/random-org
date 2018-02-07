using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Community.RandomOrg.Data;
using Community.RandomOrg.Tests.Internal;
using Community.RandomOrg.Tests.Resources;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Community.RandomOrg.Tests
{
    public sealed partial class RandomOrgClientTests
    {
        [Fact]
        public void ConstructorWhenApiKeyFormatIsInvalid()
        {
            Assert.ThrowsAny<ArgumentException>(() =>
                new RandomOrgClient("XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX"));
        }

        [Theory]
        [InlineData(00000, +0000000000, +0000000005)]
        [InlineData(10001, +0000000000, +0000000005)]
        [InlineData(00001, -1000000001, +0000000005)]
        [InlineData(00001, +1000000001, +0000000005)]
        [InlineData(00001, +0000000000, -1000000001)]
        [InlineData(00001, +0000000000, +1000000001)]
        public async void GenerateIntegersWithInvalidParameter(int count, int minimum, int maximum)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_int_req.json"));

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpMessageInvoker()))
            {
                await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                    client.GenerateIntegersAsync(count, minimum, maximum, false));
            }
        }

        [Fact]
        public async void GenerateIntegers()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_int_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_int_res.json"));

            var joparams = joreq["params"];
            var jorandom = jores["result"]["random"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpMessageInvoker(joreq, jores)))
            {
                var result = await client.GenerateIntegersAsync(
                    joparams["n"].ToObject<int>(),
                    joparams["min"].ToObject<int>(),
                    joparams["max"].ToObject<int>(),
                    joparams["replacement"].ToObject<bool>());

                VerifyResult(result, jores);

                Assert.Equal(jorandom["data"].ToObject<int[]>(), result.Random.Data);
            }
        }

        [Theory]
        [InlineData(00000, +0000000000, +0000000005)]
        [InlineData(10001, +0000000000, +0000000005)]
        [InlineData(00001, -1000000001, +0000000005)]
        [InlineData(00001, +1000000001, +0000000005)]
        [InlineData(00001, +0000000000, -1000000001)]
        [InlineData(00001, +0000000000, +1000000001)]
        public async void GenerateIntegerSequenceWithInvalidParameter(int count, int minimum, int maximum)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_seq_req.json"));

            var counts = new[] { count };
            var minimums = new[] { minimum };
            var maximums = new[] { maximum };
            var replacements = new[] { false };

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpMessageInvoker()))
            {
                await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                    client.GenerateIntegerSequencesAsync(counts, minimums, maximums, replacements));
            }
        }

        [Theory]
        [InlineData(0000, 0000000001)]
        [InlineData(1001, 0000000001)]
        [InlineData(0002, 1000000000)]
        public async void GenerateIntegerSequenceWithInvalidCount(int sequencesCount, int sequnceCount)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_seq_req.json"));

            var counts = new int[sequencesCount];
            var minimums = new int[sequencesCount];
            var maximums = new int[sequencesCount];
            var replacements = new bool[sequencesCount];

            for (var i = 0; i < sequencesCount; i++)
            {
                counts[i] = sequnceCount;
                minimums[i] = 1;
                maximums[i] = 5;
                replacements[i] = false;
            }

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpMessageInvoker()))
            {
                await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                    client.GenerateIntegerSequencesAsync(counts, minimums, maximums, replacements));
            }
        }

        [Fact]
        public async void GenerateIntegerSequences()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_seq_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_seq_res.json"));

            var joparams = joreq["params"];
            var jorandom = jores["result"]["random"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpMessageInvoker(joreq, jores)))
            {
                var result = await client.GenerateIntegerSequencesAsync(
                    joparams["length"].ToObject<int[]>(),
                    joparams["min"].ToObject<int[]>(),
                    joparams["max"].ToObject<int[]>(),
                    joparams["replacement"].ToObject<bool[]>());

                VerifyResult(result, jores);

                Assert.Equal(jorandom["data"].ToObject<int[][]>(), result.Random.Data);
            }
        }

        [Theory]
        [InlineData(00000, 02)]
        [InlineData(10001, 02)]
        [InlineData(00001, 00)]
        [InlineData(00001, 21)]
        public async void GenerateDecimalFractionsWithInvalidParameter(int count, int decimalPlaces)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_dfr_req.json"));

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpMessageInvoker()))
            {
                await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                    client.GenerateDecimalFractionsAsync(count, decimalPlaces, false));
            }
        }

        [Fact]
        public async void GenerateDecimalFractions()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_dfr_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_dfr_res.json"));

            var joparams = joreq["params"];
            var jorandom = jores["result"]["random"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpMessageInvoker(joreq, jores)))
            {
                var result = await client.GenerateDecimalFractionsAsync(
                    joparams["n"].ToObject<int>(),
                    joparams["decimalPlaces"].ToObject<int>(),
                    joparams["replacement"].ToObject<bool>());

                VerifyResult(result, jores);

                Assert.Equal(jorandom["data"].ToObject<decimal[]>(), result.Random.Data);
            }
        }

        [Theory]
        [InlineData(00000, "+0000000.0", "+0000000.0", 02)]
        [InlineData(10001, "+0000000.0", "+0000000.0", 02)]
        [InlineData(00001, "-1000001.0", "+0000000.0", 02)]
        [InlineData(00001, "+1000001.0", "+0000000.0", 02)]
        [InlineData(00001, "+0000000.0", "-1000001.0", 02)]
        [InlineData(00001, "+0000000.0", "+1000001.0", 02)]
        [InlineData(00001, "+0000000.0", "+0000000.0", 01)]
        [InlineData(00001, "+0000000.0", "+0000000.0", 21)]
        public async void GenerateGaussiansWithInvalidParameter(int count, string mean, string standardDeviation, int significantDigits)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_gss_req.json"));

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpMessageInvoker()))
            {
                var meanValue = decimal.Parse(mean, CultureInfo.InvariantCulture);
                var standardDeviationValue = decimal.Parse(standardDeviation, CultureInfo.InvariantCulture);

                await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                    client.GenerateGaussiansAsync(count, meanValue, standardDeviationValue, significantDigits));
            }
        }

        [Fact]
        public async void GenerateGaussians()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_gss_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_gss_res.json"));

            var joparams = joreq["params"];
            var jorandom = jores["result"]["random"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpMessageInvoker(joreq, jores)))
            {
                var result = await client.GenerateGaussiansAsync(
                    joparams["n"].ToObject<int>(),
                    joparams["mean"].ToObject<decimal>(),
                    joparams["standardDeviation"].ToObject<decimal>(),
                    joparams["significantDigits"].ToObject<int>());

                VerifyResult(result, jores);

                Assert.Equal(jorandom["data"].ToObject<decimal[]>(), result.Random.Data);
            }
        }

        [Theory]
        [InlineData(00000, 01, +01)]
        [InlineData(10001, 01, +01)]
        [InlineData(00001, 00, +01)]
        [InlineData(00001, 21, +01)]
        [InlineData(00001, 01, -01)]
        [InlineData(00001, 01, +00)]
        [InlineData(00001, 01, +81)]
        public async void GenerateStringsWithInvalidParameter(int count, int length, int charactersCount)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_str_req.json"));
            var characters = CreateTestString(charactersCount);

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpMessageInvoker()))
            {
                await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                    client.GenerateStringsAsync(count, length, characters, false));
            }
        }

        [Fact]
        public async void GenerateStrings()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_str_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_str_res.json"));

            var joparams = joreq["params"];
            var jorandom = jores["result"]["random"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpMessageInvoker(joreq, jores)))
            {
                var result = await client.GenerateStringsAsync(
                    joparams["n"].ToObject<int>(),
                    joparams["length"].ToObject<int>(),
                    joparams["characters"].ToObject<string>(),
                    joparams["replacement"].ToObject<bool>());

                VerifyResult(result, jores);

                Assert.Equal(jorandom["data"].ToObject<string[]>(), result.Random.Data);
            }
        }

        [Theory]
        [InlineData(0000)]
        [InlineData(1001)]
        public async void GenerateUuidsWithInvalidParameter(int count)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_uid_req.json"));

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpMessageInvoker()))
            {
                await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                    client.GenerateUuidsAsync(count));
            }
        }

        [Fact]
        public async void GenerateUuids()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_uid_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_uid_res.json"));

            var joparams = joreq["params"];
            var jorandom = jores["result"]["random"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpMessageInvoker(joreq, jores)))
            {
                var result = await client.GenerateUuidsAsync(
                    joparams["n"].ToObject<int>());

                VerifyResult(result, jores);

                Assert.Equal(jorandom["data"].ToObject<Guid[]>(), result.Random.Data);
            }
        }

        [Theory]
        [InlineData(000, 000001)]
        [InlineData(101, 000001)]
        [InlineData(001, 000000)]
        [InlineData(001, 131073)]
        [InlineData(002, 131072)]
        public async void GenerateBlobsWithInvalidParameter(int count, int size)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_blb_req.json"));

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpMessageInvoker()))
            {
                await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                    client.GenerateBlobsAsync(count, size));
            }
        }

        [Fact]
        public async void GenerateBlobs()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_blb_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_blb_res.json"));

            var joparams = joreq["params"];
            var jorandom = jores["result"]["random"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpMessageInvoker(joreq, jores)))
            {
                var result = await client.GenerateBlobsAsync(
                    joparams["n"].ToObject<int>(),
                    joparams["size"].ToObject<int>() / 8);

                VerifyResult(result, jores);

                var jodata = (JArray)jorandom["data"];

                for (var i = 0; i < jodata.Count; i++)
                {
                    Assert.Equal(Convert.FromBase64String(jodata[i].ToObject<string>()), result.Random.Data[i]);
                }
            }
        }

        [Fact]
        public async void GetUsageWhenApiKeyIsNull()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.get_usg_req.json"));

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpMessageInvoker()))
            {
                await Assert.ThrowsAsync<InvalidOperationException>(() =>
                    client.GetUsageAsync());
            }
        }

        [Fact]
        public async void GetUsage()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.get_usg_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.get_usg_res.json"));

            var joresult = jores["result"];
            var key = joreq["params"]["apiKey"].ToString();

            using (var client = new RandomOrgClient(key, CreateHttpMessageInvoker(joreq, jores)))
            {
                var result = await client.GetUsageAsync();

                Assert.NotNull(result);
                Assert.Equal(RandomOrgConvert.ToApiKeyStatus(joresult["status"].ToObject<string>()), result.Status);
                Assert.Equal(joresult["bitsLeft"].ToObject<long>(), result.BitsLeft);
                Assert.Equal(joresult["requestsLeft"].ToObject<long>(), result.RequestsLeft);
            }
        }

        [Fact]
        public async void GetUsageWithCancelledToken()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.get_usg_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.get_usg_res.json"));

            var key = joreq["params"]["apiKey"].ToString();

            using (var client = new RandomOrgClient(key, CreateHttpMessageInvoker(joreq, jores)))
            {
                var cancellationTokenSource = new CancellationTokenSource();

                cancellationTokenSource.Cancel();

                await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                    client.GetUsageAsync(cancellationTokenSource.Token));
            }
        }

        [Theory]
        [InlineData(00000, +0000000000, +0000000005, -0001)]
        [InlineData(10001, +0000000000, +0000000005, -0001)]
        [InlineData(00001, -1000000001, +0000000005, -0001)]
        [InlineData(00001, +1000000001, +0000000005, -0001)]
        [InlineData(00001, +0000000000, -1000000001, -0001)]
        [InlineData(00001, +0000000000, +1000000001, -0001)]
        [InlineData(00001, +0000000001, +0000000001, +1001)]
        public async void GenerateSignedIntegersWithInvalidParameter(int count, int minimum, int maximum, int userDataLength)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_int_req.json"));
            var userData = CreateTestString(userDataLength);

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpMessageInvoker()))
            {
                await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                    client.GenerateSignedIntegersAsync(count, minimum, maximum, false, userData));
            }
        }

        [Fact]
        public async void GenerateSignedIntegers()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_int_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_int_res.json"));

            var joparams = joreq["params"];
            var jorandom = jores["result"]["random"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpMessageInvoker(joreq, jores)))
            {
                var result = await client.GenerateSignedIntegersAsync(
                    joparams["n"].ToObject<int>(),
                    joparams["min"].ToObject<int>(),
                    joparams["max"].ToObject<int>(),
                    joparams["replacement"].ToObject<bool>(),
                    joparams["userData"].ToObject<string>());

                VerifyResult(result, jores);

                Assert.Equal(jorandom["n"].ToObject<int>(), result.Random.Data.Count);
                Assert.Equal(jorandom["data"].ToObject<int[]>(), result.Random.Data);
                Assert.Equal(jorandom["min"].ToObject<int>(), result.Random.Parameters.Minimum);
                Assert.Equal(jorandom["max"].ToObject<int>(), result.Random.Parameters.Maximum);
                Assert.Equal(jorandom["replacement"].ToObject<bool>(), result.Random.Parameters.Replacement);
                Assert.Equal(jorandom["userData"].ToObject<string>(), result.Random.UserData);
            }
        }

        [Theory]
        [InlineData(00000, +0000000000, +0000000005, -0001)]
        [InlineData(10001, +0000000000, +0000000005, -0001)]
        [InlineData(00001, -1000000001, +0000000005, -0001)]
        [InlineData(00001, +1000000001, +0000000005, -0001)]
        [InlineData(00001, +0000000000, -1000000001, -0001)]
        [InlineData(00001, +0000000000, +1000000001, -0001)]
        [InlineData(00001, +0000000001, +0000000001, +1001)]
        public async void GenerateSignedIntegerSequenceWithInvalidParameter(int count, int minimum, int maximum, int userDataLength)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_seq_req.json"));

            var counts = new[] { count };
            var minimums = new[] { minimum };
            var maximums = new[] { maximum };
            var replacements = new[] { false };

            var userData = CreateTestString(userDataLength);

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpMessageInvoker()))
            {
                await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                    client.GenerateSignedIntegerSequencesAsync(counts, minimums, maximums, replacements, userData));
            }
        }

        [Theory]
        [InlineData(0000, 0000000001)]
        [InlineData(1001, 0000000001)]
        [InlineData(0002, 1000000000)]
        public async void GenerateSignedIntegerSequenceWithInvalidCount(int sequencesCount, int sequnceCount)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_bas_seq_req.json"));

            var counts = new int[sequencesCount];
            var minimums = new int[sequencesCount];
            var maximums = new int[sequencesCount];
            var replacements = new bool[sequencesCount];

            for (var i = 0; i < sequencesCount; i++)
            {
                counts[i] = sequnceCount;
                minimums[i] = 1;
                maximums[i] = 5;
                replacements[i] = false;
            }

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpMessageInvoker()))
            {
                await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                    client.GenerateSignedIntegerSequencesAsync(counts, minimums, maximums, replacements));
            }
        }

        [Fact]
        public async void GenerateSignedIntegerSequences()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_seq_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_seq_res.json"));

            var joparams = joreq["params"];
            var jorandom = jores["result"]["random"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpMessageInvoker(joreq, jores)))
            {
                var result = await client.GenerateSignedIntegerSequencesAsync(
                    joparams["length"].ToObject<int[]>(),
                    joparams["min"].ToObject<int[]>(),
                    joparams["max"].ToObject<int[]>(),
                    joparams["replacement"].ToObject<bool[]>(),
                    joparams["userData"].ToObject<string>());

                VerifyResult(result, jores);

                Assert.Equal(jorandom["n"].ToObject<int>(), result.Random.Data.Count);
                Assert.Equal(jorandom["length"].ToObject<int[]>(), result.Random.Data.Select(s => s.Count));
                Assert.Equal(jorandom["data"].ToObject<int[][]>(), result.Random.Data);
                Assert.Equal(jorandom["min"].ToObject<int[]>(), result.Random.Parameters.Minimums);
                Assert.Equal(jorandom["max"].ToObject<int[]>(), result.Random.Parameters.Maximums);
                Assert.Equal(jorandom["replacement"].ToObject<bool[]>(), result.Random.Parameters.Replacements);
                Assert.Equal(jorandom["userData"].ToObject<string>(), result.Random.UserData);
            }
        }

        [Theory]
        [InlineData(00000, 02, -0001)]
        [InlineData(10001, 02, -0001)]
        [InlineData(00001, 00, -0001)]
        [InlineData(00001, 21, -0001)]
        [InlineData(00001, 01, +1001)]
        public async void GenerateSignedDecimalFractionsWithInvalidParameter(int count, int decimalPlaces, int userDataLength)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_dfr_req.json"));
            var userData = CreateTestString(userDataLength);

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpMessageInvoker()))
            {
                await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                    client.GenerateSignedDecimalFractionsAsync(count, decimalPlaces, false, userData));
            }
        }

        [Fact]
        public async void GenerateSignedDecimalFractions()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_dfr_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_dfr_res.json"));

            var joparams = joreq["params"];
            var jorandom = jores["result"]["random"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpMessageInvoker(joreq, jores)))
            {
                var result = await client.GenerateSignedDecimalFractionsAsync(
                    joparams["n"].ToObject<int>(),
                    joparams["decimalPlaces"].ToObject<int>(),
                    joparams["replacement"].ToObject<bool>(),
                    joparams["userData"].ToObject<string>());

                VerifyResult(result, jores);

                Assert.Equal(jorandom["n"].ToObject<int>(), result.Random.Data.Count);
                Assert.Equal(jorandom["data"].ToObject<decimal[]>(), result.Random.Data);
                Assert.Equal(jorandom["decimalPlaces"].ToObject<int>(), result.Random.Parameters.DecimalPlaces);
                Assert.Equal(jorandom["replacement"].ToObject<bool>(), result.Random.Parameters.Replacement);
                Assert.Equal(jorandom["userData"].ToObject<string>(), result.Random.UserData);
            }
        }

        [Theory]
        [InlineData(00000, "+0000000.0", "+0000000.0", 02, -0001)]
        [InlineData(10001, "+0000000.0", "+0000000.0", 02, -0001)]
        [InlineData(00001, "-1000001.0", "+0000000.0", 02, -0001)]
        [InlineData(00001, "+1000001.0", "+0000000.0", 02, -0001)]
        [InlineData(00001, "+0000000.0", "-1000001.0", 02, -0001)]
        [InlineData(00001, "+0000000.0", "+1000001.0", 02, -0001)]
        [InlineData(00001, "+0000000.0", "+0000000.0", 01, -0001)]
        [InlineData(00001, "+0000000.0", "+0000000.0", 21, -0001)]
        [InlineData(00001, "+0000000.0", "+0000000.0", 02, +1001)]
        public async void GenerateSignedGaussiansWithInvalidParameter(int count, string mean, string standardDeviation, int significantDigits, int userDataLength)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_gss_req.json"));
            var userData = CreateTestString(userDataLength);

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpMessageInvoker()))
            {
                var meanValue = decimal.Parse(mean, CultureInfo.InvariantCulture);
                var standardDeviationValue = decimal.Parse(standardDeviation, CultureInfo.InvariantCulture);

                await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                    client.GenerateSignedGaussiansAsync(count, meanValue, standardDeviationValue, significantDigits, userData));
            }
        }

        [Fact]
        public async void GenerateSignedGaussians()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_gss_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_gss_res.json"));

            var joparams = joreq["params"];
            var jorandom = jores["result"]["random"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpMessageInvoker(joreq, jores)))
            {
                var result = await client.GenerateSignedGaussiansAsync(
                    joparams["n"].ToObject<int>(),
                    joparams["mean"].ToObject<decimal>(),
                    joparams["standardDeviation"].ToObject<decimal>(),
                    joparams["significantDigits"].ToObject<int>(),
                    joparams["userData"].ToObject<string>());

                VerifyResult(result, jores);

                Assert.Equal(jorandom["n"].ToObject<int>(), result.Random.Data.Count);
                Assert.Equal(jorandom["data"].ToObject<decimal[]>(), result.Random.Data);
                Assert.Equal(jorandom["mean"].ToObject<decimal>(), result.Random.Parameters.Mean);
                Assert.Equal(jorandom["standardDeviation"].ToObject<decimal>(), result.Random.Parameters.StandardDeviation);
                Assert.Equal(jorandom["significantDigits"].ToObject<int>(), result.Random.Parameters.SignificantDigits);
                Assert.Equal(jorandom["userData"].ToObject<string>(), result.Random.UserData);
            }
        }

        [Theory]
        [InlineData(00000, 01, +01, -0001)]
        [InlineData(10001, 01, +01, -0001)]
        [InlineData(00001, 00, +01, -0001)]
        [InlineData(00001, 21, +01, -0001)]
        [InlineData(00001, 01, -01, -0001)]
        [InlineData(00001, 01, +00, -0001)]
        [InlineData(00001, 01, +81, -0001)]
        [InlineData(00001, 01, +01, +1001)]
        public async void GenerateSignedStringsWithInvalidParameter(int count, int length, int charactersCount, int userDataLength)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_str_req.json"));
            var characters = CreateTestString(charactersCount);
            var userData = CreateTestString(userDataLength);

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpMessageInvoker()))
            {
                await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                    client.GenerateSignedStringsAsync(count, length, characters, false, userData));
            }
        }

        [Fact]
        public async void GenerateSignedStrings()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_str_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_str_res.json"));

            var joparams = joreq["params"];
            var jorandom = jores["result"]["random"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpMessageInvoker(joreq, jores)))
            {
                var result = await client.GenerateSignedStringsAsync(
                    joparams["n"].ToObject<int>(),
                    joparams["length"].ToObject<int>(),
                    joparams["characters"].ToObject<string>(),
                    joparams["replacement"].ToObject<bool>(),
                    joparams["userData"].ToObject<string>());

                VerifyResult(result, jores);

                Assert.Equal(jorandom["n"].ToObject<int>(), result.Random.Data.Count);
                Assert.Equal(jorandom["data"].ToObject<string[]>(), result.Random.Data);
                Assert.Equal(jorandom["length"].ToObject<int>(), result.Random.Parameters.Length);
                Assert.Equal(jorandom["characters"].ToObject<string>(), result.Random.Parameters.Characters);
                Assert.Equal(jorandom["replacement"].ToObject<bool>(), result.Random.Parameters.Replacement);
                Assert.Equal(jorandom["userData"].ToObject<string>(), result.Random.UserData);
            }
        }

        [Theory]
        [InlineData(0000, -0001)]
        [InlineData(1001, -0001)]
        [InlineData(0001, +1001)]
        public async void GenerateSignedUuidsWithInvalidParameter(int count, int userDataLength)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_uid_req.json"));
            var userData = CreateTestString(userDataLength);

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpMessageInvoker()))
            {
                await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                    client.GenerateSignedUuidsAsync(count, userData));
            }
        }

        [Fact]
        public async void GenerateSignedUuids()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_uid_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_uid_res.json"));

            var joparams = joreq["params"];
            var jorandom = jores["result"]["random"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpMessageInvoker(joreq, jores)))
            {
                var result = await client.GenerateSignedUuidsAsync(
                    joparams["n"].ToObject<int>(),
                    joparams["userData"].ToObject<string>());

                VerifyResult(result, jores);

                Assert.Equal(jorandom["n"].ToObject<int>(), result.Random.Data.Count);
                Assert.Equal(jorandom["data"].ToObject<Guid[]>(), result.Random.Data);
                Assert.Equal(jorandom["userData"].ToObject<string>(), result.Random.UserData);
            }
        }

        [Theory]
        [InlineData(000, 000001, -0001)]
        [InlineData(101, 000001, -0001)]
        [InlineData(001, 000000, -0001)]
        [InlineData(001, 131073, -0001)]
        [InlineData(002, 131072, -0001)]
        [InlineData(001, 000001, +1001)]
        public async void GenerateSignedBlobsWithInvalidParameter(int count, int size, int userDataLength)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_blb_req.json"));
            var userData = CreateTestString(userDataLength);

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpMessageInvoker()))
            {
                await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                    client.GenerateSignedBlobsAsync(count, size, userData));
            }
        }

        [Fact]
        public async void GenerateSignedBlobs()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_blb_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_blb_res.json"));

            var joparams = joreq["params"];
            var jorandom = jores["result"]["random"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpMessageInvoker(joreq, jores)))
            {
                var result = await client.GenerateSignedBlobsAsync(
                    joparams["n"].ToObject<int>(),
                    joparams["size"].ToObject<int>() / 8,
                    joparams["userData"].ToObject<string>());

                VerifyResult(result, jores);

                Assert.Equal(jorandom["n"].ToObject<int>(), result.Random.Data.Count);

                var jodata = (JArray)jorandom["data"];

                for (var i = 0; i < jodata.Count; i++)
                {
                    Assert.Equal(Convert.FromBase64String(jodata[i].ToObject<string>()), result.Random.Data[i]);
                }

                Assert.Equal(jorandom["size"].ToObject<int>(), result.Random.Parameters.Size * 8);
                Assert.Equal(jorandom["userData"].ToObject<string>(), result.Random.UserData);
            }
        }

        [Fact]
        public async void VerifySignatureWhenRandomIsNull()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_rin_req.json"));

            var joparams = joreq["params"];

            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateEmptyHttpMessageInvoker()))
            {
                var signature = Convert.FromBase64String(joparams["signature"].ToObject<string>());

                await Assert.ThrowsAsync<ArgumentNullException>(() =>
                    client.VerifySignatureAsync((SignedRandom<int, IntegerParameters>)null, signature));
            }
        }

        [Fact]
        public async void VerifySignatureWhenSignatureIsNull()
        {
            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateEmptyHttpMessageInvoker()))
            {
                var random = new SignedRandom<int, IntegerParameters>();

                await Assert.ThrowsAsync<ArgumentNullException>(() =>
                    client.VerifySignatureAsync(random, (byte[])null));
            }
        }

        [Fact]
        public async void VerifySignatureWhenLicenseTypeIsNull()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_ltn_req.json"));

            var joparams = joreq["params"];
            var jorandom = joreq["params"]["random"];
            var jolicense = joreq["params"]["random"]["license"];

            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateEmptyHttpMessageInvoker()))
            {
                var random = new SignedRandom<int, IntegerParameters>
                {
                    ApiKeyHash = Convert.FromBase64String(jorandom["hashedApiKey"].ToObject<string>()),
                    CompletionTime = jorandom["completionTime"].ToObject<DateTime>(),
                    SerialNumber = jorandom["serialNumber"].ToObject<int>(),
                    Data = jorandom["data"].ToObject<int[]>(),
                    UserData = jorandom["userData"].ToObject<string>()
                };

                random.Parameters.Minimum = jorandom["min"].ToObject<int>();
                random.Parameters.Maximum = jorandom["max"].ToObject<int>();
                random.Parameters.Replacement = jorandom["replacement"].ToObject<bool>();
                random.License.Type = jolicense["type"].ToObject<string>();
                random.License.Text = jolicense["text"].ToObject<string>();
                random.License.InfoUrl = new Uri(jolicense["infoUrl"].ToObject<string>());

                var signature = Convert.FromBase64String(joparams["signature"].ToObject<string>());

                await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                    client.VerifySignatureAsync(random, signature));
            }
        }

        [Fact]
        public async void VerifySignatureForIntegers()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_int_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_int_res.json"));

            var joparams = joreq["params"];
            var jorandom = joreq["params"]["random"];
            var jolicense = joreq["params"]["random"]["license"];

            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateHttpMessageInvoker(joreq, jores)))
            {
                var random = new SignedRandom<int, IntegerParameters>
                {
                    ApiKeyHash = Convert.FromBase64String(jorandom["hashedApiKey"].ToObject<string>()),
                    CompletionTime = jorandom["completionTime"].ToObject<DateTime>(),
                    SerialNumber = jorandom["serialNumber"].ToObject<int>(),
                    Data = jorandom["data"].ToObject<int[]>(),
                    UserData = jorandom["userData"].ToObject<string>()
                };

                random.Parameters.Minimum = jorandom["min"].ToObject<int>();
                random.Parameters.Maximum = jorandom["max"].ToObject<int>();
                random.Parameters.Replacement = jorandom["replacement"].ToObject<bool>();
                random.License.Type = jolicense["type"].ToObject<string>();
                random.License.Text = jolicense["text"].ToObject<string>();
                random.License.InfoUrl = new Uri(jolicense["infoUrl"].ToObject<string>());

                var signature = Convert.FromBase64String(joparams["signature"].ToObject<string>());
                var result = await client.VerifySignatureAsync(random, signature);

                Assert.Equal(result, jores["result"]["authenticity"].ToObject<bool>());
            }
        }

        [Fact]
        public async void VerifySignatureForIntegerSequencesWhenSequenceIsNull()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_seq_req.json"));

            var joparams = joreq["params"];
            var jorandom = joreq["params"]["random"];
            var jolicense = joreq["params"]["random"]["license"];

            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateEmptyHttpMessageInvoker()))
            {
                var random = new SignedRandom<IReadOnlyList<int>, IntegerSequenceParameters>
                {
                    ApiKeyHash = Convert.FromBase64String(jorandom["hashedApiKey"].ToObject<string>()),
                    CompletionTime = jorandom["completionTime"].ToObject<DateTime>(),
                    SerialNumber = jorandom["serialNumber"].ToObject<int>(),
                    Data = new IReadOnlyList<int>[jorandom["data"].ToObject<IReadOnlyList<int>[]>().Length],
                    UserData = jorandom["userData"].ToObject<string>()
                };

                random.Parameters.Minimums = jorandom["min"].ToObject<int[]>();
                random.Parameters.Maximums = jorandom["max"].ToObject<int[]>();
                random.Parameters.Replacements = jorandom["replacement"].ToObject<bool[]>();
                random.License.Type = jolicense["type"].ToObject<string>();
                random.License.Text = jolicense["text"].ToObject<string>();
                random.License.InfoUrl = new Uri(jolicense["infoUrl"].ToObject<string>());

                var signature = Convert.FromBase64String(joparams["signature"].ToObject<string>());

                await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                    client.VerifySignatureAsync(random, signature));
            }
        }

        [Fact]
        public async void VerifySignatureForIntegerSequencesWhenMinimumsIsNull()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_seq_req.json"));

            var joparams = joreq["params"];
            var jorandom = joreq["params"]["random"];
            var jolicense = joreq["params"]["random"]["license"];

            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateEmptyHttpMessageInvoker()))
            {
                var random = new SignedRandom<IReadOnlyList<int>, IntegerSequenceParameters>
                {
                    ApiKeyHash = Convert.FromBase64String(jorandom["hashedApiKey"].ToObject<string>()),
                    CompletionTime = jorandom["completionTime"].ToObject<DateTime>(),
                    SerialNumber = jorandom["serialNumber"].ToObject<int>(),
                    Data = jorandom["data"].ToObject<IReadOnlyList<int>[]>(),
                    UserData = jorandom["userData"].ToObject<string>()
                };

                random.Parameters.Minimums = null;
                random.Parameters.Maximums = jorandom["max"].ToObject<int[]>();
                random.Parameters.Replacements = jorandom["replacement"].ToObject<bool[]>();
                random.License.Type = jolicense["type"].ToObject<string>();
                random.License.Text = jolicense["text"].ToObject<string>();
                random.License.InfoUrl = new Uri(jolicense["infoUrl"].ToObject<string>());

                var signature = Convert.FromBase64String(joparams["signature"].ToObject<string>());

                await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                    client.VerifySignatureAsync(random, signature));
            }
        }

        [Fact]
        public async void VerifySignatureForIntegerSequencesWhenMaximumsIsNull()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_seq_req.json"));

            var joparams = joreq["params"];
            var jorandom = joreq["params"]["random"];
            var jolicense = joreq["params"]["random"]["license"];

            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateEmptyHttpMessageInvoker()))
            {
                var random = new SignedRandom<IReadOnlyList<int>, IntegerSequenceParameters>
                {
                    ApiKeyHash = Convert.FromBase64String(jorandom["hashedApiKey"].ToObject<string>()),
                    CompletionTime = jorandom["completionTime"].ToObject<DateTime>(),
                    SerialNumber = jorandom["serialNumber"].ToObject<int>(),
                    Data = jorandom["data"].ToObject<IReadOnlyList<int>[]>(),
                    UserData = jorandom["userData"].ToObject<string>()
                };

                random.Parameters.Minimums = jorandom["min"].ToObject<int[]>();
                random.Parameters.Maximums = null;
                random.Parameters.Replacements = jorandom["replacement"].ToObject<bool[]>();
                random.License.Type = jolicense["type"].ToObject<string>();
                random.License.Text = jolicense["text"].ToObject<string>();
                random.License.InfoUrl = new Uri(jolicense["infoUrl"].ToObject<string>());

                var signature = Convert.FromBase64String(joparams["signature"].ToObject<string>());

                await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                    client.VerifySignatureAsync(random, signature));
            }
        }

        [Fact]
        public async void VerifySignatureForIntegerSequencesWhenReplacementsIsNull()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_seq_req.json"));

            var joparams = joreq["params"];
            var jorandom = joreq["params"]["random"];
            var jolicense = joreq["params"]["random"]["license"];

            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateEmptyHttpMessageInvoker()))
            {
                var random = new SignedRandom<IReadOnlyList<int>, IntegerSequenceParameters>
                {
                    ApiKeyHash = Convert.FromBase64String(jorandom["hashedApiKey"].ToObject<string>()),
                    CompletionTime = jorandom["completionTime"].ToObject<DateTime>(),
                    SerialNumber = jorandom["serialNumber"].ToObject<int>(),
                    Data = jorandom["data"].ToObject<IReadOnlyList<int>[]>(),
                    UserData = jorandom["userData"].ToObject<string>()
                };

                random.Parameters.Minimums = jorandom["min"].ToObject<int[]>();
                random.Parameters.Maximums = jorandom["max"].ToObject<int[]>();
                random.Parameters.Replacements = null;
                random.License.Type = jolicense["type"].ToObject<string>();
                random.License.Text = jolicense["text"].ToObject<string>();
                random.License.InfoUrl = new Uri(jolicense["infoUrl"].ToObject<string>());

                var signature = Convert.FromBase64String(joparams["signature"].ToObject<string>());

                await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                    client.VerifySignatureAsync(random, signature));
            }
        }

        [Theory]
        [InlineData(1, 0, 0)]
        [InlineData(0, 1, 0)]
        [InlineData(0, 0, 1)]
        public async void VerifySignatureForIntegerSequencesWithInvalidCount(int minimumsDelta, int maximumsDelta, int replacementsDelta)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_seq_req.json"));

            var joparams = joreq["params"];
            var jorandom = joreq["params"]["random"];
            var jolicense = joreq["params"]["random"]["license"];

            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateEmptyHttpMessageInvoker()))
            {
                var random = new SignedRandom<IReadOnlyList<int>, IntegerSequenceParameters>
                {
                    ApiKeyHash = Convert.FromBase64String(jorandom["hashedApiKey"].ToObject<string>()),
                    CompletionTime = jorandom["completionTime"].ToObject<DateTime>(),
                    SerialNumber = jorandom["serialNumber"].ToObject<int>(),
                    Data = jorandom["data"].ToObject<IReadOnlyList<int>[]>(),
                    UserData = jorandom["userData"].ToObject<string>()
                };

                random.Parameters.Minimums = new int[jorandom["min"].ToObject<int[]>().Length + minimumsDelta];
                random.Parameters.Maximums = new int[jorandom["max"].ToObject<int[]>().Length + maximumsDelta];
                random.Parameters.Replacements = new bool[jorandom["replacement"].ToObject<bool[]>().Length + replacementsDelta];
                random.License.Type = jolicense["type"].ToObject<string>();
                random.License.Text = jolicense["text"].ToObject<string>();
                random.License.InfoUrl = new Uri(jolicense["infoUrl"].ToObject<string>());

                var signature = Convert.FromBase64String(joparams["signature"].ToObject<string>());

                await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                    client.VerifySignatureAsync(random, signature));
            }
        }

        [Fact]
        public async void VerifySignatureForIntegerSequences()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_seq_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_seq_res.json"));

            var joparams = joreq["params"];
            var jorandom = joreq["params"]["random"];
            var jolicense = joreq["params"]["random"]["license"];

            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateHttpMessageInvoker(joreq, jores)))
            {
                var random = new SignedRandom<IReadOnlyList<int>, IntegerSequenceParameters>
                {
                    ApiKeyHash = Convert.FromBase64String(jorandom["hashedApiKey"].ToObject<string>()),
                    CompletionTime = jorandom["completionTime"].ToObject<DateTime>(),
                    SerialNumber = jorandom["serialNumber"].ToObject<int>(),
                    Data = jorandom["data"].ToObject<IReadOnlyList<int>[]>(),
                    UserData = jorandom["userData"].ToObject<string>()
                };

                random.Parameters.Minimums = jorandom["min"].ToObject<int[]>();
                random.Parameters.Maximums = jorandom["max"].ToObject<int[]>();
                random.Parameters.Replacements = jorandom["replacement"].ToObject<bool[]>();
                random.License.Type = jolicense["type"].ToObject<string>();
                random.License.Text = jolicense["text"].ToObject<string>();
                random.License.InfoUrl = new Uri(jolicense["infoUrl"].ToObject<string>());

                var signature = Convert.FromBase64String(joparams["signature"].ToObject<string>());
                var result = await client.VerifySignatureAsync(random, signature);

                Assert.Equal(result, jores["result"]["authenticity"].ToObject<bool>());
            }
        }

        [Fact]
        public async void VerifySignatureForDecimalFractions()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_dfr_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_dfr_res.json"));

            var joparams = joreq["params"];
            var jorandom = joreq["params"]["random"];
            var jolicense = joreq["params"]["random"]["license"];

            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateHttpMessageInvoker(joreq, jores)))
            {
                var random = new SignedRandom<decimal, DecimalFractionParameters>
                {
                    ApiKeyHash = Convert.FromBase64String(jorandom["hashedApiKey"].ToObject<string>()),
                    CompletionTime = jorandom["completionTime"].ToObject<DateTime>(),
                    SerialNumber = jorandom["serialNumber"].ToObject<int>(),
                    Data = jorandom["data"].ToObject<decimal[]>(),
                    UserData = jorandom["userData"].ToObject<string>()
                };

                random.Parameters.DecimalPlaces = jorandom["decimalPlaces"].ToObject<int>();
                random.Parameters.Replacement = jorandom["replacement"].ToObject<bool>();
                random.License.Type = jolicense["type"].ToObject<string>();
                random.License.Text = jolicense["text"].ToObject<string>();
                random.License.InfoUrl = new Uri(jolicense["infoUrl"].ToObject<string>());

                var signature = Convert.FromBase64String(joparams["signature"].ToObject<string>());
                var result = await client.VerifySignatureAsync(random, signature);

                Assert.Equal(result, jores["result"]["authenticity"].ToObject<bool>());
            }
        }

        [Fact]
        public async void VerifySignatureForGaussians()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_gss_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_gss_res.json"));

            var joparams = joreq["params"];
            var jorandom = joreq["params"]["random"];
            var jolicense = joreq["params"]["random"]["license"];

            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateHttpMessageInvoker(joreq, jores)))
            {
                var random = new SignedRandom<decimal, GaussianParameters>
                {
                    ApiKeyHash = Convert.FromBase64String(jorandom["hashedApiKey"].ToObject<string>()),
                    CompletionTime = jorandom["completionTime"].ToObject<DateTime>(),
                    SerialNumber = jorandom["serialNumber"].ToObject<int>(),
                    Data = jorandom["data"].ToObject<decimal[]>(),
                    UserData = jorandom["userData"].ToObject<string>()
                };

                random.Parameters.Mean = jorandom["mean"].ToObject<decimal>();
                random.Parameters.StandardDeviation = jorandom["standardDeviation"].ToObject<decimal>();
                random.Parameters.SignificantDigits = jorandom["significantDigits"].ToObject<int>();
                random.License.Type = jolicense["type"].ToObject<string>();
                random.License.Text = jolicense["text"].ToObject<string>();
                random.License.InfoUrl = new Uri(jolicense["infoUrl"].ToObject<string>());

                var signature = Convert.FromBase64String(joparams["signature"].ToObject<string>());
                var result = await client.VerifySignatureAsync(random, signature);

                Assert.Equal(result, jores["result"]["authenticity"].ToObject<bool>());
            }
        }

        [Fact]
        public async void VerifySignatureForStringsWhenCharactersIsNull()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_str_req.json"));

            var joparams = joreq["params"];
            var jorandom = joreq["params"]["random"];
            var jolicense = joreq["params"]["random"]["license"];

            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateEmptyHttpMessageInvoker()))
            {
                var random = new SignedRandom<string, StringParameters>
                {
                    ApiKeyHash = Convert.FromBase64String(jorandom["hashedApiKey"].ToObject<string>()),
                    CompletionTime = jorandom["completionTime"].ToObject<DateTime>(),
                    SerialNumber = jorandom["serialNumber"].ToObject<int>(),
                    Data = jorandom["data"].ToObject<string[]>(),
                    UserData = jorandom["userData"].ToObject<string>()
                };

                random.Parameters.Length = jorandom["length"].ToObject<int>();
                random.Parameters.Characters = null;
                random.Parameters.Replacement = jorandom["replacement"].ToObject<bool>();
                random.License.Type = jolicense["type"].ToObject<string>();
                random.License.Text = jolicense["text"].ToObject<string>();
                random.License.InfoUrl = new Uri(jolicense["infoUrl"].ToObject<string>());

                var signature = Convert.FromBase64String(joparams["signature"].ToObject<string>());

                await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                    client.VerifySignatureAsync(random, signature));
            }
        }

        [Fact]
        public async void VerifySignatureForStrings()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_str_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_str_res.json"));

            var joparams = joreq["params"];
            var jorandom = joreq["params"]["random"];
            var jolicense = joreq["params"]["random"]["license"];

            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateHttpMessageInvoker(joreq, jores)))
            {
                var random = new SignedRandom<string, StringParameters>
                {
                    ApiKeyHash = Convert.FromBase64String(jorandom["hashedApiKey"].ToObject<string>()),
                    CompletionTime = jorandom["completionTime"].ToObject<DateTime>(),
                    SerialNumber = jorandom["serialNumber"].ToObject<int>(),
                    Data = jorandom["data"].ToObject<string[]>(),
                    UserData = jorandom["userData"].ToObject<string>()
                };

                random.Parameters.Length = jorandom["length"].ToObject<int>();
                random.Parameters.Characters = jorandom["characters"].ToObject<string>();
                random.Parameters.Replacement = jorandom["replacement"].ToObject<bool>();
                random.License.Type = jolicense["type"].ToObject<string>();
                random.License.Text = jolicense["text"].ToObject<string>();
                random.License.InfoUrl = new Uri(jolicense["infoUrl"].ToObject<string>());

                var signature = Convert.FromBase64String(joparams["signature"].ToObject<string>());
                var result = await client.VerifySignatureAsync(random, signature);

                Assert.Equal(result, jores["result"]["authenticity"].ToObject<bool>());
            }
        }

        [Fact]
        public async void VerifySignatureForUuids()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_uid_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_uid_res.json"));

            var joparams = joreq["params"];
            var jorandom = joreq["params"]["random"];
            var jolicense = joreq["params"]["random"]["license"];

            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateHttpMessageInvoker(joreq, jores)))
            {
                var random = new SignedRandom<Guid, UuidParameters>
                {
                    ApiKeyHash = Convert.FromBase64String(jorandom["hashedApiKey"].ToObject<string>()),
                    CompletionTime = jorandom["completionTime"].ToObject<DateTime>(),
                    SerialNumber = jorandom["serialNumber"].ToObject<int>(),
                    Data = jorandom["data"].ToObject<Guid[]>(),
                    UserData = jorandom["userData"].ToObject<string>()
                };

                random.License.Type = jolicense["type"].ToObject<string>();
                random.License.Text = jolicense["text"].ToObject<string>();
                random.License.InfoUrl = new Uri(jolicense["infoUrl"].ToObject<string>());

                var signature = Convert.FromBase64String(joparams["signature"].ToObject<string>());
                var result = await client.VerifySignatureAsync(random, signature);

                Assert.Equal(result, jores["result"]["authenticity"].ToObject<bool>());
            }
        }

        [Fact]
        public async void VerifySignatureForBlobs()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_blb_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_blb_res.json"));

            var joparams = joreq["params"];
            var jorandom = joreq["params"]["random"];
            var jolicense = joreq["params"]["random"]["license"];

            var jodata = (JArray)jorandom["data"];
            var data = new byte[jodata.Count][];

            for (var i = 0; i < jodata.Count; i++)
            {
                data[i] = Convert.FromBase64String(jodata[i].ToObject<string>());
            }

            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateHttpMessageInvoker(joreq, jores)))
            {
                var random = new SignedRandom<byte[], BlobParameters>
                {
                    ApiKeyHash = Convert.FromBase64String(jorandom["hashedApiKey"].ToObject<string>()),
                    CompletionTime = jorandom["completionTime"].ToObject<DateTime>(),
                    SerialNumber = jorandom["serialNumber"].ToObject<int>(),
                    Data = data,
                    UserData = jorandom["userData"].ToObject<string>()
                };

                random.Parameters.Size = jorandom["size"].ToObject<int>() / 8;
                random.License.Type = jolicense["type"].ToObject<string>();
                random.License.Text = jolicense["text"].ToObject<string>();
                random.License.InfoUrl = new Uri(jolicense["infoUrl"].ToObject<string>());

                var signature = Convert.FromBase64String(joparams["signature"].ToObject<string>());
                var result = await client.VerifySignatureAsync(random, signature);

                Assert.Equal(result, jores["result"]["authenticity"].ToObject<bool>());
            }
        }

        [Fact]
        public async void GetUsageWhenHttpStatusCodeIsInvalid()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.get_usg_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.get_usg_res.json"));

            var key = joreq["params"]["apiKey"].ToString();

            async Task<HttpResponseMessage> Handler(HttpRequestMessage request)
            {
                var joreqa = JObject.Parse(await request.Content.ReadAsStringAsync().ConfigureAwait(false));

                joreq["id"] = joreqa["id"];
                jores["id"] = joreqa["id"];

                Assert.True(JToken.DeepEquals(joreqa, joreq));

                var content = new StringContent(jores.ToString());

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = content
                };
            }

            using (var client = new RandomOrgClient(key, new HttpClient(new TestHttpMessageHandler(_output, Handler))))
            {
                var exception = await Assert.ThrowsAsync<RandomOrgRequestException>(() =>
                    client.GetUsageAsync());

                Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
            }
        }

        [Theory]
        [InlineData("get_usg_101")]
        [InlineData("get_usg_501")]
        public async void GetUsageWithServiceError(string test)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString($"Assets.{test}_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString($"Assets.{test}_res.json"));

            var joparams = joreq["params"];
            var joerror = jores["error"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpMessageInvoker(joreq, jores)))
            {
                var exception = await Assert.ThrowsAsync<RandomOrgException>(() =>
                    client.GetUsageAsync());

                Assert.Equal(joreq["method"].ToObject<string>(), exception.Method);
                Assert.Equal(joerror["code"].ToObject<long>(), exception.Code);
                Assert.Equal(joerror["message"].ToObject<string>(), exception.Message);
            }
        }
    }
}