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
        private readonly IDictionary<string, RandomOrgClient> _clients = new Dictionary<string, RandomOrgClient>(StringComparer.Ordinal);

        protected RandomOrgClientBenchmarks()
        {
            var assets = new[]
            {
                "bas_blb",
                "bas_dfr",
                "bas_gss",
                "bas_int",
                "bas_seq",
                "bas_str",
                "bas_uid",
                "sig_blb",
                "sig_dfr",
                "sig_gss",
                "sig_int",
                "sig_seq",
                "sig_str",
                "sig_uid"
            };

            foreach (var asset in assets)
            {
                var handler = new RandomOrgBenchmarkHandler(EmbeddedResourceManager.GetString($"Assets.{asset}.json"));

                _clients[asset] = new RandomOrgClient(Guid.Empty.ToString(), new HttpClient(handler));
            }
        }

        [Benchmark(Description = "BAS BLB")]
        public async Task GenerateBasicBlobsAsync()
        {
            await _clients["bas_blb"].GenerateBlobsAsync(2, 128).ConfigureAwait(false);
        }

        [Benchmark(Description = "BAS DFR")]
        public async Task GenerateBasicDecimalFractionsAsync()
        {
            await _clients["bas_dfr"].GenerateDecimalFractionsAsync(2, 8, true).ConfigureAwait(false);
        }

        [Benchmark(Description = "BAS GSS")]
        public async Task GenerateBasicGaussiansAsync()
        {
            await _clients["bas_gss"].GenerateGaussiansAsync(2, 0, 1, 8).ConfigureAwait(false);
        }

        [Benchmark(Description = "BAS INT")]
        public async Task GenerateBasicIntegersAsync()
        {
            await _clients["bas_int"].GenerateIntegersAsync(8, 1, 256, true).ConfigureAwait(false);
        }

        [Benchmark(Description = "BAS SEQ")]
        public async Task GenerateBasicIntegeGenerateIntegerSequencesAsyncrsAsync()
        {
            await _clients["bas_seq"].GenerateIntegerSequencesAsync(new[] { 3, 5 }, new[] { 1, 2 }, new[] { 128, 256 }, new[] { true, true }).ConfigureAwait(false);
        }

        [Benchmark(Description = "BAS STR")]
        public async Task GenerateBasicStringsAsync()
        {
            await _clients["bas_str"].GenerateStringsAsync(2, 16, "abcdefghijklmnopqrstuvwxyz", true).ConfigureAwait(false);
        }

        [Benchmark(Description = "BAS UID")]
        public async Task GenerateBasicUuidsAsync()
        {
            await _clients["bas_uid"].GenerateUuidsAsync(2).ConfigureAwait(false);
        }

        [Benchmark(Description = "SIG BLB")]
        public async Task GenerateSignedBlobsAsync()
        {
            await _clients["sig_blb"].GenerateSignedBlobsAsync(2, 128).ConfigureAwait(false);
        }

        [Benchmark(Description = "SIG DFR")]
        public async Task GenerateSignedDecimalFractionsAsync()
        {
            await _clients["sig_dfr"].GenerateSignedDecimalFractionsAsync(2, 8, true).ConfigureAwait(false);
        }

        [Benchmark(Description = "SIG GSS")]
        public async Task GenerateSignedGaussiansAsync()
        {
            await _clients["sig_gss"].GenerateSignedGaussiansAsync(2, 0, 1, 8).ConfigureAwait(false);
        }

        [Benchmark(Description = "SIG INT")]
        public async Task GenerateSignedIntegersAsync()
        {
            await _clients["sig_int"].GenerateSignedIntegersAsync(8, 1, 256, true).ConfigureAwait(false);
        }

        [Benchmark(Description = "SIG SEQ")]
        public async Task GenerateSignedIntegeGenerateIntegerSequencesAsyncrsAsync()
        {
            await _clients["sig_seq"].GenerateSignedIntegerSequencesAsync(new[] { 3, 5 }, new[] { 1, 2 }, new[] { 128, 256 }, new[] { true, true }).ConfigureAwait(false);
        }

        [Benchmark(Description = "SIG STR")]
        public async Task GenerateSignedStringsAsync()
        {
            await _clients["sig_str"].GenerateSignedStringsAsync(2, 16, "abcdefghijklmnopqrstuvwxyz", true).ConfigureAwait(false);
        }

        [Benchmark(Description = "SIG UID")]
        public async Task GenerateSignedUuidsAsync()
        {
            await _clients["sig_uid"].GenerateSignedUuidsAsync(2).ConfigureAwait(false);
        }
    }
}