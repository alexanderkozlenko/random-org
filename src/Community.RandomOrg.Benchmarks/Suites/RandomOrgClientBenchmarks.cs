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
                "gen_bas_blb",
                "gen_bas_dfr",
                "gen_bas_gss",
                "gen_bas_int",
                "gen_bas_seq",
                "gen_bas_str",
                "gen_bas_uid",
                "gen_sig_blb",
                "gen_sig_dfr",
                "gen_sig_gss",
                "gen_sig_int",
                "gen_sig_seq",
                "gen_sig_str",
                "gen_sig_uid"
            };

            foreach (var asset in assets)
            {
                var handler = new RandomOrgBenchmarkHandler(EmbeddedResourceManager.GetString($"Assets.{asset}.json"));

                _clients[asset] = new RandomOrgClient(Guid.Empty.ToString(), new HttpClient(handler));
            }
        }

        [Benchmark(Description = "gen_bas_blb")]
        public async Task GenerateBasicBlobsAsync()
        {
            await _clients["gen_bas_blb"].GenerateBlobsAsync(2, 128).ConfigureAwait(false);
        }

        [Benchmark(Description = "gen_bas_dfr")]
        public async Task GenerateBasicDecimalFractionsAsync()
        {
            await _clients["gen_bas_dfr"].GenerateDecimalFractionsAsync(2, 8, true).ConfigureAwait(false);
        }

        [Benchmark(Description = "gen_bas_gss")]
        public async Task GenerateBasicGaussiansAsync()
        {
            await _clients["gen_bas_gss"].GenerateGaussiansAsync(2, 0, 1, 8).ConfigureAwait(false);
        }

        [Benchmark(Description = "gen_bas_int")]
        public async Task GenerateBasicIntegersAsync()
        {
            await _clients["gen_bas_int"].GenerateIntegersAsync(8, 1, 256, true).ConfigureAwait(false);
        }

        [Benchmark(Description = "gen_bas_seq")]
        public async Task GenerateBasicIntegeGenerateIntegerSequencesAsyncrsAsync()
        {
            await _clients["gen_bas_seq"].GenerateIntegerSequencesAsync(new[] { 3, 5 }, new[] { 1, 2 }, new[] { 128, 256 }, new[] { true, true }).ConfigureAwait(false);
        }

        [Benchmark(Description = "gen_bas_str")]
        public async Task GenerateBasicStringsAsync()
        {
            await _clients["gen_bas_str"].GenerateStringsAsync(2, 16, "abcdefghijklmnopqrstuvwxyz", true).ConfigureAwait(false);
        }

        [Benchmark(Description = "gen_bas_uid")]
        public async Task GenerateBasicUuidsAsync()
        {
            await _clients["gen_bas_uid"].GenerateUuidsAsync(2).ConfigureAwait(false);
        }

        [Benchmark(Description = "gen_sig_blb")]
        public async Task GenerateSignedBlobsAsync()
        {
            await _clients["gen_sig_blb"].GenerateSignedBlobsAsync(2, 128).ConfigureAwait(false);
        }

        [Benchmark(Description = "gen_sig_dfr")]
        public async Task GenerateSignedDecimalFractionsAsync()
        {
            await _clients["gen_sig_dfr"].GenerateSignedDecimalFractionsAsync(2, 8, true).ConfigureAwait(false);
        }

        [Benchmark(Description = "gen_sig_gss")]
        public async Task GenerateSignedGaussiansAsync()
        {
            await _clients["gen_sig_gss"].GenerateSignedGaussiansAsync(2, 0, 1, 8).ConfigureAwait(false);
        }

        [Benchmark(Description = "gen_sig_int")]
        public async Task GenerateSignedIntegersAsync()
        {
            await _clients["gen_sig_int"].GenerateSignedIntegersAsync(8, 1, 256, true).ConfigureAwait(false);
        }

        [Benchmark(Description = "gen_sig_seq")]
        public async Task GenerateSignedIntegeGenerateIntegerSequencesAsyncrsAsync()
        {
            await _clients["gen_sig_seq"].GenerateSignedIntegerSequencesAsync(new[] { 3, 5 }, new[] { 1, 2 }, new[] { 128, 256 }, new[] { true, true }).ConfigureAwait(false);
        }

        [Benchmark(Description = "gen_sig_str")]
        public async Task GenerateSignedStringsAsync()
        {
            await _clients["gen_sig_str"].GenerateSignedStringsAsync(2, 16, "abcdefghijklmnopqrstuvwxyz", true).ConfigureAwait(false);
        }

        [Benchmark(Description = "gen_sig_uid")]
        public async Task GenerateSignedUuidsAsync()
        {
            await _clients["gen_sig_uid"].GenerateSignedUuidsAsync(2).ConfigureAwait(false);
        }
    }
}