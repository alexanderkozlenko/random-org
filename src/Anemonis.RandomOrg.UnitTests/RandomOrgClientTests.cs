using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Anemonis.RandomOrg.Data;
using Anemonis.RandomOrg.UnitTests.Internal;
using Anemonis.RandomOrg.UnitTests.Resources;
using Anemonis.RandomOrg.UnitTests.TestStubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Anemonis.RandomOrg.UnitTests
{
    [TestClass]
    public sealed partial class RandomOrgClientTests
    {
        [TestMethod]
        public void ConstructorWhenApiKeyFormatIsInvalid()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                new RandomOrgClient("XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX"));
        }

        [TestMethod]
        public async Task GetUsageWhenApiKeyIsNull()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.get_usg_req.json"));

            using (var client = new RandomOrgClient(joreq["params"]["apiKey"].ToString(), CreateEmptyHttpInvoker()))
            {
                await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                    client.GetUsageAsync());
            }
        }

        [TestMethod]
        public async Task GetUsage()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.get_usg_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.get_usg_res.json"));

            var joresult = jores["result"];
            var key = joreq["params"]["apiKey"].ToString();

            using (var client = new RandomOrgClient(key, CreateHttpInvoker(joreq, jores)))
            {
                var result = await client.GetUsageAsync();

                Assert.IsNotNull(result);
                Assert.AreEqual(RandomOrgConverter.StringToApiKeyStatus(joresult["status"].ToObject<string>()), result.Status);
                Assert.AreEqual(joresult["bitsLeft"].ToObject<long>(), result.BitsLeft);
                Assert.AreEqual(joresult["requestsLeft"].ToObject<long>(), result.RequestsLeft);
            }
        }

        [TestMethod]
        public async Task GetUsageWithCancelledToken()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.get_usg_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.get_usg_res.json"));

            var key = joreq["params"]["apiKey"].ToString();

            using (var client = new RandomOrgClient(key, CreateHttpInvoker(joreq, jores)))
            {
                var cancellationTokenSource = new CancellationTokenSource();

                cancellationTokenSource.Cancel();

                await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                    client.GetUsageAsync(cancellationTokenSource.Token));
            }
        }

        [TestMethod]
        public async Task GetUsageWhenHttpStatusCodeIsInvalid()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.get_usg_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.get_usg_res.json"));

            var key = joreq["params"]["apiKey"].ToString();

            async Task<HttpResponseMessage> Handler(HttpRequestMessage request)
            {
                var joreqa = JObject.Parse(await request.Content.ReadAsStringAsync().ConfigureAwait(false));

                joreq["id"] = joreqa["id"];
                jores["id"] = joreqa["id"];

                Assert.IsTrue(JToken.DeepEquals(joreqa, joreq));

                var content = new StringContent(jores.ToString());

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = content
                };
            }

            using (var client = new RandomOrgClient(key, new HttpClient(new TestHttpHandler(Handler))))
            {
                var exception = await Assert.ThrowsExceptionAsync<RandomOrgProtocolException>(() =>
                    client.GetUsageAsync());

                Assert.AreEqual(HttpStatusCode.BadRequest, exception.StatusCode);
            }
        }

        [DataTestMethod]
        [DataRow("get_usg_101")]
        [DataRow("get_usg_501")]
        public async Task GetUsageWithServiceError(string test)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString($"Assets.{test}_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString($"Assets.{test}_res.json"));

            var joparams = joreq["params"];
            var joerror = jores["error"];

            using (var client = new RandomOrgClient(joparams["apiKey"].ToString(), CreateHttpInvoker(joreq, jores)))
            {
                var exception = await Assert.ThrowsExceptionAsync<RandomOrgException>(() =>
                    client.GetUsageAsync());

                Assert.AreEqual(joreq["method"].ToObject<string>(), exception.Method);
                Assert.AreEqual(joerror["code"].ToObject<long>(), exception.Code);
                Assert.AreEqual(joerror["message"].ToObject<string>(), exception.Message);
            }
        }

        [TestMethod]
        public async Task VerifySignatureWhenRandomIsNull()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_rin_req.json"));

            var joparams = joreq["params"];

            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateEmptyHttpInvoker()))
            {
                var signature = Convert.FromBase64String(joparams["signature"].ToObject<string>());

                await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                    client.VerifySignatureAsync((SignedRandom<int, IntegerParameters>)null, signature));
            }
        }

        [TestMethod]
        public async Task VerifySignatureWhenSignatureIsNull()
        {
            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateEmptyHttpInvoker()))
            {
                var random = new SignedRandom<int, IntegerParameters>();

                await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                    client.VerifySignatureAsync(random, (byte[])null));
            }
        }

        [TestMethod]
        public async Task VerifySignatureWhenLicenseTypeIsNull()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_ltn_req.json"));

            var joparams = joreq["params"];
            var jorandom = joreq["params"]["random"];
            var jolicense = joreq["params"]["random"]["license"];

            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateEmptyHttpInvoker()))
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

                await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                    client.VerifySignatureAsync(random, signature));
            }
        }

        [TestMethod]
        public async Task VerifySignatureForIntegers()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_int_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_int_res.json"));

            var joparams = joreq["params"];
            var jorandom = joreq["params"]["random"];
            var jolicense = joreq["params"]["random"]["license"];

            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateHttpInvoker(joreq, jores)))
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

                Assert.AreEqual(result, jores["result"]["authenticity"].ToObject<bool>());
            }
        }

        [TestMethod]
        public async Task VerifySignatureForIntegerSequencesWhenSequenceIsNull()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_seq_req.json"));

            var joparams = joreq["params"];
            var jorandom = joreq["params"]["random"];
            var jolicense = joreq["params"]["random"]["license"];

            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateEmptyHttpInvoker()))
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

                await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                    client.VerifySignatureAsync(random, signature));
            }
        }

        [DataTestMethod]
        [DataRow(true, false, false)]
        [DataRow(false, true, false)]
        [DataRow(false, false, true)]
        public async Task VerifySignatureForIntegerSequencesWhenParameterIsNull(bool minimumsIsNull, bool maximumsIsNull, bool replacementsIsNull)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_seq_req.json"));

            var joparams = joreq["params"];
            var jorandom = joreq["params"]["random"];
            var jolicense = joreq["params"]["random"]["license"];

            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateEmptyHttpInvoker()))
            {
                var random = new SignedRandom<IReadOnlyList<int>, IntegerSequenceParameters>
                {
                    ApiKeyHash = Convert.FromBase64String(jorandom["hashedApiKey"].ToObject<string>()),
                    CompletionTime = jorandom["completionTime"].ToObject<DateTime>(),
                    SerialNumber = jorandom["serialNumber"].ToObject<int>(),
                    Data = jorandom["data"].ToObject<IReadOnlyList<int>[]>(),
                    UserData = jorandom["userData"].ToObject<string>()
                };

                random.Parameters.Minimums = minimumsIsNull ? null : jorandom["min"].ToObject<int[]>();
                random.Parameters.Maximums = maximumsIsNull ? null : jorandom["max"].ToObject<int[]>();
                random.Parameters.Replacements = replacementsIsNull ? null : jorandom["replacement"].ToObject<bool[]>();
                random.License.Type = jolicense["type"].ToObject<string>();
                random.License.Text = jolicense["text"].ToObject<string>();
                random.License.InfoUrl = new Uri(jolicense["infoUrl"].ToObject<string>());

                var signature = Convert.FromBase64String(joparams["signature"].ToObject<string>());

                await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                    client.VerifySignatureAsync(random, signature));
            }
        }

        [DataTestMethod]
        [DataRow(1, 0, 0)]
        [DataRow(0, 1, 0)]
        [DataRow(0, 0, 1)]
        public async Task VerifySignatureForIntegerSequencesWithInvalidCount(int minimumsDelta, int maximumsDelta, int replacementsDelta)
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_seq_req.json"));

            var joparams = joreq["params"];
            var jorandom = joreq["params"]["random"];
            var jolicense = joreq["params"]["random"]["license"];

            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateEmptyHttpInvoker()))
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

                await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                    client.VerifySignatureAsync(random, signature));
            }
        }

        [TestMethod]
        public async Task VerifySignatureForIntegerSequences()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_seq_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_seq_res.json"));

            var joparams = joreq["params"];
            var jorandom = joreq["params"]["random"];
            var jolicense = joreq["params"]["random"]["license"];

            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateHttpInvoker(joreq, jores)))
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

                Assert.AreEqual(result, jores["result"]["authenticity"].ToObject<bool>());
            }
        }

        [TestMethod]
        public async Task VerifySignatureForDecimalFractions()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_dfr_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_dfr_res.json"));

            var joparams = joreq["params"];
            var jorandom = joreq["params"]["random"];
            var jolicense = joreq["params"]["random"]["license"];

            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateHttpInvoker(joreq, jores)))
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

                Assert.AreEqual(result, jores["result"]["authenticity"].ToObject<bool>());
            }
        }

        [TestMethod]
        public async Task VerifySignatureForGaussians()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_gss_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_gss_res.json"));

            var joparams = joreq["params"];
            var jorandom = joreq["params"]["random"];
            var jolicense = joreq["params"]["random"]["license"];

            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateHttpInvoker(joreq, jores)))
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

                Assert.AreEqual(result, jores["result"]["authenticity"].ToObject<bool>());
            }
        }

        [TestMethod]
        public async Task VerifySignatureForStringsWhenCharactersIsNull()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_str_req.json"));

            var joparams = joreq["params"];
            var jorandom = joreq["params"]["random"];
            var jolicense = joreq["params"]["random"]["license"];

            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateEmptyHttpInvoker()))
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

                await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                    client.VerifySignatureAsync(random, signature));
            }
        }

        [TestMethod]
        public async Task VerifySignatureForStrings()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_str_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_str_res.json"));

            var joparams = joreq["params"];
            var jorandom = joreq["params"]["random"];
            var jolicense = joreq["params"]["random"]["license"];

            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateHttpInvoker(joreq, jores)))
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

                Assert.AreEqual(result, jores["result"]["authenticity"].ToObject<bool>());
            }
        }

        [TestMethod]
        public async Task VerifySignatureForUuids()
        {
            var joreq = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_uid_req.json"));
            var jores = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_uid_res.json"));

            var joparams = joreq["params"];
            var jorandom = joreq["params"]["random"];
            var jolicense = joreq["params"]["random"]["license"];

            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateHttpInvoker(joreq, jores)))
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

                Assert.AreEqual(result, jores["result"]["authenticity"].ToObject<bool>());
            }
        }

        [TestMethod]
        public async Task VerifySignatureForBlobs()
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

            using (var client = new RandomOrgClient(Guid.Empty.ToString(), CreateHttpInvoker(joreq, jores)))
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

                Assert.AreEqual(result, jores["result"]["authenticity"].ToObject<bool>());
            }
        }
    }
}