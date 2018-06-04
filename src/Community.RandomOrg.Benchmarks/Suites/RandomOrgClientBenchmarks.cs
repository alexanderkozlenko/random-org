using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Community.RandomOrg.Benchmarks.Internal;
using Community.RandomOrg.Benchmarks.Resources;

namespace Community.RandomOrg.Benchmarks.Suites
{
    public abstract class RandomOrgClientBenchmarks
    {
        private static readonly IReadOnlyDictionary<string, string> _resources = CreateResourceDictionary();
        private static readonly (int[] Lengths, int[] Minimums, int[] Maximums, bool[] Replacements) _integerSequenceParameters = CreateIntegerSequenceParameters();

        private readonly RandomOrgClient _client;

        protected RandomOrgClientBenchmarks()
        {
            var contents = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["generateIntegers"] = _resources["bas_int"],
                ["generateIntegerSequences"] = _resources["bas_seq"],
                ["generateDecimalFractions"] = _resources["bas_dfr"],
                ["generateGaussians"] = _resources["bas_gss"],
                ["generateStrings"] = _resources["bas_str"],
                ["generateUUIDs"] = _resources["bas_uid"],
                ["generateBlobs"] = _resources["bas_blb"],
                ["generateSignedIntegers"] = _resources["sig_int"],
                ["generateSignedIntegerSequences"] = _resources["sig_seq"],
                ["generateSignedDecimalFractions"] = _resources["sig_dfr"],
                ["generateSignedGaussians"] = _resources["sig_gss"],
                ["generateSignedStrings"] = _resources["sig_str"],
                ["generateSignedUUIDs"] = _resources["sig_uid"],
                ["generateSignedBlobs"] = _resources["sig_blb"]
            };

            _client = new RandomOrgClient("00000000-0000-0000-0000-000000000000", new HttpClient(new RandomOrgBenchmarkHandler(contents)));
        }

        private static IReadOnlyDictionary<string, string> CreateResourceDictionary()
        {
            var resources = new Dictionary<string, string>(StringComparer.Ordinal);

            foreach (var code in GetResponseCodes())
            {
                resources[code] = EmbeddedResourceManager.GetString($"Assets.{code}.json");
            }

            return resources;
        }

        private static (int[], int[], int[], bool[]) CreateIntegerSequenceParameters()
        {
            return (new[] { 3, 5 }, new[] { 1, 2 }, new[] { 128, 256 }, new[] { true, true });
        }

        private static IEnumerable<string> GetResponseCodes()
        {
            return new[]
            {
                "bas_blb", "bas_dfr", "bas_gss", "bas_int", "bas_seq", "bas_str", "bas_uid",
                "sig_blb", "sig_dfr", "sig_gss", "sig_int", "sig_seq", "sig_str", "sig_uid"
            };
        }

        [Benchmark]
        public async Task<object> GenerateBasicBlobsAsync()
        {
            return await _client.GenerateBlobsAsync(2, 128);
        }

        [Benchmark]
        public async Task<object> GenerateBasicDecimalFractionsAsync()
        {
            return await _client.GenerateDecimalFractionsAsync(2, 8, true);
        }

        [Benchmark]
        public async Task<object> GenerateBasicGaussiansAsync()
        {
            return await _client.GenerateGaussiansAsync(2, 0, 1, 8);
        }

        [Benchmark]
        public async Task<object> GenerateBasicIntegersAsync()
        {
            return await _client.GenerateIntegersAsync(8, 1, 256, true);
        }

        [Benchmark]
        public async Task<object> GenerateBasicIntegeGenerateIntegerSequencesAsyncrsAsync()
        {
            return await _client.GenerateIntegerSequencesAsync(
                _integerSequenceParameters.Lengths,
                _integerSequenceParameters.Minimums,
                _integerSequenceParameters.Maximums,
                _integerSequenceParameters.Replacements);
        }

        [Benchmark]
        public async Task<object> GenerateBasicStringsAsync()
        {
            return await _client.GenerateStringsAsync(2, 16, "01234567abcdefgh", true);
        }

        [Benchmark]
        public async Task<object> GenerateBasicUuidsAsync()
        {
            return await _client.GenerateUuidsAsync(2);
        }

        [Benchmark]
        public async Task<object> GenerateSignedBlobsAsync()
        {
            return await _client.GenerateSignedBlobsAsync(2, 128);
        }

        [Benchmark]
        public async Task<object> GenerateSignedDecimalFractionsAsync()
        {
            return await _client.GenerateSignedDecimalFractionsAsync(2, 8, true);
        }

        [Benchmark]
        public async Task<object> GenerateSignedGaussiansAsync()
        {
            return await _client.GenerateSignedGaussiansAsync(2, 0, 1, 8);
        }

        [Benchmark]
        public async Task<object> GenerateSignedIntegersAsync()
        {
            return await _client.GenerateSignedIntegersAsync(8, 1, 256, true);
        }

        [Benchmark]
        public async Task<object> GenerateSignedIntegeGenerateIntegerSequencesAsyncrsAsync()
        {
            return await _client.GenerateSignedIntegerSequencesAsync(
                _integerSequenceParameters.Lengths,
                _integerSequenceParameters.Minimums,
                _integerSequenceParameters.Maximums,
                _integerSequenceParameters.Replacements);
        }

        [Benchmark]
        public async Task<object> GenerateSignedStringsAsync()
        {
            return await _client.GenerateSignedStringsAsync(2, 16, "01234567abcdefgh", true);
        }

        [Benchmark]
        public async Task<object> GenerateSignedUuidsAsync()
        {
            return await _client.GenerateSignedUuidsAsync(2);
        }
    }
}