using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Community.RandomOrg.Data;
using Community.RandomOrg.Tests.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Community.RandomOrg.Tests
{
    partial class RandomOrgClientTests
    {
        private readonly ITestOutputHelper _output;

        public RandomOrgClientTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private static string CreateTestString(int length)
        {
            return length >= 0 ? new string('*', length) : null;
        }

        private async Task<HttpResponseMessage> HandleRequest(HttpRequestMessage request, JObject joreq, JObject jores)
        {
            var joreqa = JObject.Parse(await request.Content.ReadAsStringAsync().ConfigureAwait(false));

            joreq["id"] = joreqa["id"];
            jores["id"] = joreqa["id"];

            _output.WriteLine(joreqa.ToString(Formatting.Indented));

            Assert.True(JToken.DeepEquals(joreqa, joreq), "Actual JSON string differs from expected");

            var content = new StringContent(jores.ToString());

            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = content
            };
        }

        private HttpClient CreateHttpInvoker(JObject joreq, JObject jores)
        {
            return new HttpClient(new TestHttpHandler(_output, request => HandleRequest(request, joreq, jores)));
        }

        private HttpClient CreateEmptyHttpInvoker()
        {
            return new HttpClient(new TestHttpHandler());
        }

        private static void VerifyResult<TValue>(RandomResult<TValue> result, JObject jores)
        {
            Assert.NotNull(result);
            Assert.NotNull(result.Random);
            Assert.NotNull(result.Random.Data);

            var joresult = jores["result"];
            var jorandom = jores["result"]["random"];

            Assert.Equal(joresult["bitsUsed"].ToObject<long>(), result.BitsUsed);
            Assert.Equal(joresult["bitsLeft"].ToObject<long>(), result.BitsLeft);
            Assert.Equal(joresult["requestsLeft"].ToObject<long>(), result.RequestsLeft);
            Assert.Equal(RandomOrgConvert.ToDateTime(jorandom["completionTime"].ToObject<string>()), result.Random.CompletionTime);
        }

        private static void VerifyResult<TValue, TParameters>(SignedRandomResult<TValue, TParameters> result, JObject jores)
            where TParameters : RandomParameters, new()
        {
            Assert.NotNull(result);
            Assert.NotNull(result.Random);
            Assert.NotNull(result.Random.Data);
            Assert.NotNull(result.Random.Parameters);
            Assert.NotNull(result.Random.License);

            var joresult = jores["result"];
            var jorandom = jores["result"]["random"];
            var jolicense = jores["result"]["random"]["license"];

            Assert.Equal(joresult["bitsUsed"].ToObject<long>(), result.BitsUsed);
            Assert.Equal(joresult["bitsLeft"].ToObject<long>(), result.BitsLeft);
            Assert.Equal(joresult["requestsLeft"].ToObject<long>(), result.RequestsLeft);
            Assert.Equal(Convert.FromBase64String(joresult["signature"].ToObject<string>()), result.Signature);
            Assert.Equal(RandomOrgConvert.ToDateTime(jorandom["completionTime"].ToObject<string>()), result.Random.CompletionTime);
            Assert.Equal(Convert.FromBase64String(jorandom["hashedApiKey"].ToObject<string>()), result.Random.ApiKeyHash);
            Assert.Equal(jorandom["serialNumber"].ToObject<long>(), result.Random.SerialNumber);
            Assert.Equal(jorandom["userData"].ToObject<string>(), result.Random.UserData);
            Assert.Equal(jolicense["type"].ToObject<string>(), result.Random.License.Type);
            Assert.Equal(jolicense["text"].ToObject<string>(), result.Random.License.Text);
            Assert.Equal(jolicense["infoUrl"].ToObject<string>(), result.Random.License.InfoUrl?.OriginalString);
        }
   }
}