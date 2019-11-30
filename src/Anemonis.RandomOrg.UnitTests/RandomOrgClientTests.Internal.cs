using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Anemonis.RandomOrg.Data;
using Anemonis.RandomOrg.UnitTests.Internal;
using Anemonis.RandomOrg.UnitTests.TestStubs;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Anemonis.RandomOrg.UnitTests
{
    public partial class RandomOrgClientTests
    {
        [Conditional("DEBUG")]
        private static void TraceJsonObject(JObject @object)
        {
            Trace.WriteLine(@object.ToString(Formatting.Indented));
        }

        private static string CreateTestString(int length)
        {
            return length >= 0 ? new string('*', length) : null;
        }

        private async Task<HttpResponseMessage> HandleRequest(HttpRequestMessage request, JObject joreq, JObject jores)
        {
            Assert.IsTrue(request.Headers.Contains("Accept"));
            Assert.IsTrue(request.Headers.Contains("Accept-Charset"));
            Assert.IsTrue(request.Headers.Contains("Date"));
            Assert.IsTrue(request.Headers.Contains("User-Agent"));

            var joreqa = JObject.Parse(await request.Content.ReadAsStringAsync());

            TraceJsonObject(joreqa);

            joreq["id"] = joreqa["id"];
            jores["id"] = joreqa["id"];

            Assert.IsTrue(JToken.DeepEquals(joreqa, joreq), "Actual JSON string differs from expected");

            var content = new StringContent(jores.ToString());

            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = content
            };
        }

        private HttpClient CreateHttpInvoker(JObject joreq, JObject jores)
        {
            return new HttpClient(new TestHttpHandler(request => HandleRequest(request, joreq, jores)));
        }

        private HttpClient CreateEmptyHttpInvoker()
        {
            return new HttpClient(new TestHttpHandler());
        }

        private static void VerifyResult<TValue>(RandomResult<TValue> result, JObject jores)
        {
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Random);
            Assert.IsNotNull(result.Random.Data);

            var joresult = jores["result"];
            var jorandom = jores["result"]["random"];

            Assert.AreEqual(joresult["bitsUsed"].ToObject<long>(), result.BitsUsed);
            Assert.AreEqual(joresult["bitsLeft"].ToObject<long>(), result.BitsLeft);
            Assert.AreEqual(joresult["requestsLeft"].ToObject<long>(), result.RequestsLeft);
            Assert.AreEqual(RandomOrgConverter.StringToDateTime(jorandom["completionTime"].ToObject<string>()), result.Random.CompletionTime);
        }

        private static void VerifyResult<TValue, TParameters>(SignedRandomResult<TValue, TParameters> result, JObject jores)
            where TParameters : RandomParameters, new()
        {
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Random);
            Assert.IsNotNull(result.Random.Data);
            Assert.IsNotNull(result.Random.Parameters);
            Assert.IsNotNull(result.Random.License);

            var joresult = jores["result"];
            var jorandom = jores["result"]["random"];
            var jolicense = jores["result"]["random"]["license"];

            Assert.AreEqual(joresult["bitsUsed"].ToObject<long>(), result.BitsUsed);
            Assert.AreEqual(joresult["bitsLeft"].ToObject<long>(), result.BitsLeft);
            Assert.AreEqual(joresult["requestsLeft"].ToObject<long>(), result.RequestsLeft);

            CollectionAssert.AreEqual(Convert.FromBase64String(joresult["signature"].ToObject<string>()), result.GetSignature());

            Assert.AreEqual(RandomOrgConverter.StringToDateTime(jorandom["completionTime"].ToObject<string>()), result.Random.CompletionTime);

            CollectionAssert.AreEqual(Convert.FromBase64String(jorandom["hashedApiKey"].ToObject<string>()), result.Random.ApiKeyHash?.ToArray());

            Assert.AreEqual(jorandom["serialNumber"].ToObject<long>(), result.Random.SerialNumber);
            Assert.AreEqual(jorandom["userData"].ToObject<string>(), result.Random.UserData);
            Assert.AreEqual(jolicense["type"].ToObject<string>(), result.Random.License.Type);
            Assert.AreEqual(jolicense["text"].ToObject<string>(), result.Random.License.Text);
            Assert.AreEqual(jolicense["infoUrl"].ToObject<string>(), result.Random.License.InfoUrl?.OriginalString);
        }
    }
}
