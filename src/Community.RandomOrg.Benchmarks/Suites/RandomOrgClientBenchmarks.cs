using System;
using System.Collections.Generic;
using System.Data.JsonRpc;
using System.Net.Http;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Community.RandomOrg.Benchmarks.Framework;
using Community.RandomOrg.Benchmarks.Internal;
using Community.RandomOrg.Benchmarks.Resources;

namespace Community.RandomOrg.Benchmarks.Suites
{
    [BenchmarkSuite(nameof(JsonRpcSerializer))]
    public abstract class RandomOrgClientBenchmarks
    {
        private const string _apiKey = "00000000-0000-0000-0000-000000000000";

        private readonly IReadOnlyDictionary<string, HttpMessageInvoker> _invokers;

        protected RandomOrgClientBenchmarks()
        {
            var assets = new[]
            {
                "bas_blb", "bas_dfr", "bas_gss", "bas_int", "bas_seq", "bas_str", "bas_uid",
                "sig_blb", "sig_dfr", "sig_gss", "sig_int", "sig_seq", "sig_str", "sig_uid"
            };

            var clients = new Dictionary<string, HttpMessageInvoker>(assets.Length, StringComparer.Ordinal);

            foreach (var asset in assets)
            {
                clients[asset] = new HttpClient(new RandomOrgBenchmarkHandler(EmbeddedResourceManager.GetString($"Assets.{asset}.json")));
            }

            _invokers = clients;
        }

        [Benchmark(Description = "bas-blb")]
        public async Task GenerateBasicBlobsAsync()
        {
            await new RandomOrgClient(_apiKey, _invokers["bas_blb"])
                .GenerateBlobsAsync(2, 128)
                .ConfigureAwait(false);
        }

        [Benchmark(Description = "bas-dfr")]
        public async Task GenerateBasicDecimalFractionsAsync()
        {
            await new RandomOrgClient(_apiKey, _invokers["bas_dfr"])
                .GenerateDecimalFractionsAsync(2, 8, true)
                .ConfigureAwait(false);
        }

        [Benchmark(Description = "bas-gss")]
        public async Task GenerateBasicGaussiansAsync()
        {
            await new RandomOrgClient(_apiKey, _invokers["bas_gss"])
                .GenerateGaussiansAsync(2, 0, 1, 8)
                .ConfigureAwait(false);
        }

        [Benchmark(Description = "bas-int")]
        public async Task GenerateBasicIntegersAsync()
        {
            await new RandomOrgClient(_apiKey, _invokers["bas_int"])
                .GenerateIntegersAsync(8, 1, 256, true)
                .ConfigureAwait(false);
        }

        [Benchmark(Description = "bas-seq")]
        public async Task GenerateBasicIntegeGenerateIntegerSequencesAsyncrsAsync()
        {
            await new RandomOrgClient(_apiKey, _invokers["bas_seq"])
                .GenerateIntegerSequencesAsync(new[] { 3, 5 }, new[] { 1, 2 }, new[] { 128, 256 }, new[] { true, true })
                .ConfigureAwait(false);
        }

        [Benchmark(Description = "bas-str")]
        public async Task GenerateBasicStringsAsync()
        {
            await new RandomOrgClient(_apiKey, _invokers["bas_str"])
                .GenerateStringsAsync(2, 16, "01234567abcdefgh", true)
                .ConfigureAwait(false);
        }

        [Benchmark(Description = "bas-uid")]
        public async Task GenerateBasicUuidsAsync()
        {
            await new RandomOrgClient(_apiKey, _invokers["bas_uid"])
                .GenerateUuidsAsync(2)
                .ConfigureAwait(false);
        }

        [Benchmark(Description = "sig-blb")]
        public async Task GenerateSignedBlobsAsync()
        {
            await new RandomOrgClient(_apiKey, _invokers["sig_blb"])
                .GenerateSignedBlobsAsync(2, 128)
                .ConfigureAwait(false);
        }

        [Benchmark(Description = "sig-dfr")]
        public async Task GenerateSignedDecimalFractionsAsync()
        {
            await new RandomOrgClient(_apiKey, _invokers["sig_dfr"])
                .GenerateSignedDecimalFractionsAsync(2, 8, true)
                .ConfigureAwait(false);
        }

        [Benchmark(Description = "sig-gss")]
        public async Task GenerateSignedGaussiansAsync()
        {
            await new RandomOrgClient(_apiKey, _invokers["sig_gss"])
                .GenerateSignedGaussiansAsync(2, 0, 1, 8)
                .ConfigureAwait(false);
        }

        [Benchmark(Description = "sig-int")]
        public async Task GenerateSignedIntegersAsync()
        {
            await new RandomOrgClient(_apiKey, _invokers["sig_int"])
                .GenerateSignedIntegersAsync(8, 1, 256, true)
                .ConfigureAwait(false);
        }

        [Benchmark(Description = "sig-seq")]
        public async Task GenerateSignedIntegeGenerateIntegerSequencesAsyncrsAsync()
        {
            await new RandomOrgClient(_apiKey, _invokers["sig_seq"])
                .GenerateSignedIntegerSequencesAsync(new[] { 3, 5 }, new[] { 1, 2 }, new[] { 128, 256 }, new[] { true, true })
                .ConfigureAwait(false);
        }

        [Benchmark(Description = "sig-str")]
        public async Task GenerateSignedStringsAsync()
        {
            await new RandomOrgClient(_apiKey, _invokers["sig_str"])
                .GenerateSignedStringsAsync(2, 16, "01234567abcdefgh", true)
                .ConfigureAwait(false);
        }

        [Benchmark(Description = "sig-uid")]
        public async Task GenerateSignedUuidsAsync()
        {
            await new RandomOrgClient(_apiKey, _invokers["sig_uid"])
                .GenerateSignedUuidsAsync(2)
                .ConfigureAwait(false);
        }
    }
}