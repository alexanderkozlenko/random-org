﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Community.RandomOrg.Data;
using Community.RandomOrg.Internal;
using Community.RandomOrg.Resources;

namespace Community.RandomOrg
{
    partial class RandomOrgClient
    {
        /// <summary>Generates true random integers within a user-defined range as an asynchronous operation.</summary>
        /// <param name="count">How many random integers to generate. Must be within the [1,10000] range.</param>
        /// <param name="minimum">The lower boundary for the range from which the random numbers will be picked. Must be within the [-1000000000,1000000000] range.</param>
        /// <param name="maximum">The upper boundary for the range from which the random numbers will be picked. Must be within the [-1000000000,1000000000] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A <see cref="RandomResult{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="minimum" />, or <paramref name="maximum" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<RandomResult<int>> GenerateIntegersAsync(
            int count, int minimum, int maximum, bool replacement, CancellationToken cancellationToken)
        {
            if ((count < 1) || (count > 10000))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, Strings.GetString("random.integer.count.invalid_range"));
            }
            if ((minimum < -1000000000) || (minimum > 1000000000))
            {
                throw new ArgumentOutOfRangeException(nameof(minimum), minimum, Strings.GetString("random.integer.lower_boundary.invalid_range"));
            }
            if ((maximum < -1000000000) || (maximum > 1000000000))
            {
                throw new ArgumentOutOfRangeException(nameof(maximum), maximum, Strings.GetString("random.integer.upper_boundary.invalid_range"));
            }

            var parameters = new Dictionary<string, object>(5, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey,
                ["n"] = count,
                ["min"] = minimum,
                ["max"] = maximum,
                ["replacement"] = replacement,
                ["base"] = 10
            };

            var response = await InvokeGenerationServiceMethodAsync<RpcRandomResult<int>, RpcRandom<int>, int>(
                "generateIntegers", parameters, cancellationToken).ConfigureAwait(false);

            var random = new Random<int>
            {
                Data = response.Random.Data,
                CompletionTime = response.Random.CompletionTime
            };

            return new RandomResult<int>(
                random, response.BitsUsed, response.BitsLeft, response.RequestsLeft);
        }

        /// <summary>Generates true random integers within a user-defined range as an asynchronous operation.</summary>
        /// <param name="counts">An array specifying the lengths of the requested sequences. Each value must be within the [1,10000] range and the total sum of all the lengths must be in the [1,10000] range. Up to 10 sequences can be requested.</param>
        /// <param name="minimums">An array specifying the lower boundaries for the requested sequences. Each value must be within the [-1000000000,1000000000] range.</param>
        /// <param name="maximums">An array specifying the upper boundaries for the requested sequences. Each value must be within the [-1000000000,1000000000] range.</param>
        /// <param name="replacements">An array specifying for each requested sequence whether the random numbers in that seqeuence should be picked with replacement.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A <see cref="RandomResult{T}" /> instance.</returns>
        /// <exception cref="ArgumentException">Counts of argument arrays are different, sequences count is greater than 10, or total count is outside the [1,10000] range.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="counts" />, <paramref name="minimums" />, <paramref name="maximums" />, or <paramref name="replacements" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">One of the values from the arguments <paramref name="counts" />, <paramref name="minimums" />, or <paramref name="maximums" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<RandomResult<int[]>> GenerateIntegerSequencesAsync(
            IReadOnlyList<int> counts, IReadOnlyList<int> minimums, IReadOnlyList<int> maximums, IReadOnlyList<bool> replacements, CancellationToken cancellationToken)
        {
            if (counts == null)
            {
                throw new ArgumentNullException(nameof(counts));
            }
            if (minimums == null)
            {
                throw new ArgumentNullException(nameof(minimums));
            }
            if (maximums == null)
            {
                throw new ArgumentNullException(nameof(maximums));
            }
            if (replacements == null)
            {
                throw new ArgumentNullException(nameof(replacements));
            }

            if ((counts.Count != minimums.Count) ||
                (counts.Count != maximums.Count) ||
                (counts.Count != replacements.Count))
            {
                throw new ArgumentException(Strings.GetString("random.sequence.arguments.different_counts"));
            }
            if (counts.Count > 10)
            {
                throw new ArgumentException(Strings.GetString("random.sequence.count.invalid_range"));
            }

            var count = 0;
            var bases = new int[counts.Count];

            for (var i = 0; i < bases.Length; i++)
            {
                if ((counts[i] < 1) || (counts[i] > 10000))
                {
                    throw new ArgumentOutOfRangeException(nameof(counts) + "." + i, counts[i], Strings.GetString("random.integer.count.invalid_range"));
                }
                if ((minimums[i] < -1000000000) || (minimums[i] > 1000000000))
                {
                    throw new ArgumentOutOfRangeException(nameof(minimums) + "." + i, minimums[i], Strings.GetString("random.integer.lower_boundary.invalid_range"));
                }
                if ((maximums[i] < -1000000000) || (maximums[i] > 1000000000))
                {
                    throw new ArgumentOutOfRangeException(nameof(maximums) + "." + i, maximums[i], Strings.GetString("random.integer.upper_boundary.invalid_range"));
                }

                count += counts[i];
                bases[i] = 10;
            }

            if ((count < 1) || (count > 10000))
            {
                throw new ArgumentException(Strings.GetString("random.sequence.count.invalid_value"));
            }

            var parameters = new Dictionary<string, object>(5, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey,
                ["n"] = counts,
                ["min"] = minimums,
                ["max"] = maximums,
                ["replacement"] = replacements,
                ["base"] = bases
            };

            var response = await InvokeGenerationServiceMethodAsync<RpcRandomResult<int[]>, RpcRandom<int[]>, int[]>(
                "generateIntegerSequences", parameters, cancellationToken).ConfigureAwait(false);

            var random = new Random<int[]>
            {
                Data = response.Random.Data,
                CompletionTime = response.Random.CompletionTime
            };

            return new RandomResult<int[]>(
                random, response.BitsUsed, response.BitsLeft, response.RequestsLeft);
        }

        /// <summary>Generates true random decimal fractions from a uniform distribution across the [0,1] interval with a user-defined number of decimal places as an asynchronous operation.</summary>
        /// <param name="count">How many random decimal fractions to generate. Must be within the [1,10000] range.</param>
        /// <param name="decimalPlaces">The number of decimal places to use. Must be within the [1,20] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A <see cref="RandomResult{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="decimalPlaces" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<RandomResult<decimal>> GenerateDecimalFractionsAsync(
            int count, int decimalPlaces, bool replacement, CancellationToken cancellationToken)
        {
            if ((count < 1) || (count > 10000))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, Strings.GetString("random.decimal_fraction.count.invalid_range"));
            }
            if ((decimalPlaces < 1) || (decimalPlaces > 20))
            {
                throw new ArgumentOutOfRangeException(nameof(decimalPlaces), decimalPlaces, Strings.GetString("random.decimal_fraction.decimal_places.invalid_range"));
            }

            var parameters = new Dictionary<string, object>(4, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey,
                ["n"] = count,
                ["decimalPlaces"] = decimalPlaces,
                ["replacement"] = replacement
            };

            var response = await InvokeGenerationServiceMethodAsync<RpcRandomResult<decimal>, RpcRandom<decimal>, decimal>(
                "generateDecimalFractions", parameters, cancellationToken).ConfigureAwait(false);

            var random = new Random<decimal>
            {
                Data = response.Random.Data,
                CompletionTime = response.Random.CompletionTime
            };

            return new RandomResult<decimal>(
                random, response.BitsUsed, response.BitsLeft, response.RequestsLeft);
        }

        /// <summary>Generates true random numbers from a Gaussian distribution as an asynchronous operation.</summary>
        /// <param name="count">How many random numbers to generate. Must be within the [1,10000] range.</param>
        /// <param name="mean">The distribution's mean. Must be within the [-1000000,1000000] range.</param>
        /// <param name="standardDeviation">The distribution's standard deviation. Must be within the [-1000000,1000000] range.</param>
        /// <param name="significantDigits">The number of significant digits to use. Must be within the [2,20] range.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A <see cref="RandomResult{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="mean" />, <paramref name="standardDeviation" />, or <paramref name="significantDigits" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<RandomResult<decimal>> GenerateGaussiansAsync(
            int count, decimal mean, decimal standardDeviation, int significantDigits, CancellationToken cancellationToken)
        {
            if ((count < 1) || (count > 10000))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, Strings.GetString("random.gaussian.count.invalid_range"));
            }
            if ((mean < -1000000) || (mean > 1000000))
            {
                throw new ArgumentOutOfRangeException(nameof(mean), mean, Strings.GetString("random.gaussian.mean.invalid_range"));
            }
            if ((standardDeviation < -1000000) || (standardDeviation > 1000000))
            {
                throw new ArgumentOutOfRangeException(nameof(standardDeviation), standardDeviation, Strings.GetString("random.gaussian.standard_deviation.invalid_range"));
            }
            if ((significantDigits < 2) || (significantDigits > 20))
            {
                throw new ArgumentOutOfRangeException(nameof(significantDigits), significantDigits, Strings.GetString("random.gaussian.significant_digits.invalid_range"));
            }

            var parameters = new Dictionary<string, object>(5, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey,
                ["n"] = count,
                ["mean"] = RandomOrgConvert.DecimalToObject(mean),
                ["standardDeviation"] = RandomOrgConvert.DecimalToObject(standardDeviation),
                ["significantDigits"] = significantDigits
            };

            var response = await InvokeGenerationServiceMethodAsync<RpcRandomResult<decimal>, RpcRandom<decimal>, decimal>(
                "generateGaussians", parameters, cancellationToken).ConfigureAwait(false);

            var random = new Random<decimal>
            {
                Data = response.Random.Data,
                CompletionTime = response.Random.CompletionTime
            };

            return new RandomResult<decimal>(
                random, response.BitsUsed, response.BitsLeft, response.RequestsLeft);
        }

        /// <summary>Generates true random strings as an asynchronous operation.</summary>
        /// <param name="count">How many random strings to generate. Must be within the [1,10000] range.</param>
        /// <param name="length">The length of each string. Must be within the [1,20] range.</param>
        /// <param name="characters">A string that contains the set of characters that are allowed to occur in the random strings. The maximum number of characters is 80.</param>
        /// <param name="replacement">Specifies whether the random strings should be picked with replacement.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A <see cref="RandomResult{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="length" /> is outside the allowable range of values.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="characters" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="characters" /> contains invalid number of characters .</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<RandomResult<string>> GenerateStringsAsync(
            int count, int length, string characters, bool replacement, CancellationToken cancellationToken)
        {
            if ((count < 1) || (count > 10000))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, Strings.GetString("random.string.count.invalid_range"));
            }
            if ((length < 1) || (length > 20))
            {
                throw new ArgumentOutOfRangeException(nameof(length), length, Strings.GetString("random.string.length.invalid_range"));
            }
            if (characters == null)
            {
                throw new ArgumentNullException(nameof(characters));
            }
            if ((characters.Length < 1) || (characters.Length > 80))
            {
                throw new ArgumentException(Strings.GetString("random.string.characters.length.invalid_range"), nameof(characters));
            }

            var parameters = new Dictionary<string, object>(5, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey,
                ["n"] = count,
                ["length"] = length,
                ["characters"] = characters,
                ["replacement"] = replacement
            };

            var response = await InvokeGenerationServiceMethodAsync<RpcRandomResult<string>, RpcRandom<string>, string>(
                "generateStrings", parameters, cancellationToken).ConfigureAwait(false);

            var random = new Random<string>
            {
                Data = response.Random.Data,
                CompletionTime = response.Random.CompletionTime
            };

            return new RandomResult<string>(
                random, response.BitsUsed, response.BitsLeft, response.RequestsLeft);
        }

        /// <summary>Generates version 4 true random UUIDs in accordance with section 4.4 of RFC 4122 as an asynchronous operation.</summary>
        /// <param name="count">How many random UUIDs to generate. Must be within the [1,1000] range.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A <see cref="RandomResult{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<RandomResult<Guid>> GenerateUuidsAsync(
            int count, CancellationToken cancellationToken)
        {
            if ((count < 1) || (count > 1000))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, Strings.GetString("random.uuid.count.invalid_range"));
            }

            var parameters = new Dictionary<string, object>(2, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey,
                ["n"] = count
            };

            var response = await InvokeGenerationServiceMethodAsync<RpcRandomResult<Guid>, RpcRandom<Guid>, Guid>(
                "generateUUIDs", parameters, cancellationToken).ConfigureAwait(false);

            var random = new Random<Guid>
            {
                Data = response.Random.Data,
                CompletionTime = response.Random.CompletionTime
            };

            return new RandomResult<Guid>(
                random, response.BitsUsed, response.BitsLeft, response.RequestsLeft);
        }

        /// <summary>Generates BLOBs containing true random data as an asynchronous operation.</summary>
        /// <param name="count">How many random blobs to generate. Must be within the [1,100] range.</param>
        /// <param name="size">The size of each blob, measured in bits. Must be within the [1,1048576] range and must be divisible by 8. The total size of all blobs requested must not exceed 1048576 bits.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A <see cref="RandomResult{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="size" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<RandomResult<byte[]>> GenerateBlobsAsync(
            int count, int size, CancellationToken cancellationToken)
        {
            if ((count < 1) || (count > 100))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, Strings.GetString("random.blob.count.invalid_range"));
            }
            if ((size < 1) || (size > 1048576))
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, Strings.GetString("random.blob.size.invalid_range"));
            }
            if (size % 8 != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, Strings.GetString("random.blob.size.invalid_division"));
            }
            if (count * size > 1048576)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, Strings.GetString("random.blob.invalid_total_size"));
            }

            var parameters = new Dictionary<string, object>(4, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey,
                ["n"] = count,
                ["size"] = size,
                ["format"] = "base64"
            };

            var response = await InvokeGenerationServiceMethodAsync<RpcRandomResult<string>, RpcRandom<string>, string>(
                "generateBlobs", parameters, cancellationToken).ConfigureAwait(false);

            var data = new byte[response.Random.Data.Count][];

            for (var i = 0; i < data.Length; i++)
            {
                if (response.Random.Data[i] != null)
                {
                    data[i] = Convert.FromBase64String(response.Random.Data[i]);
                }
            }

            var random = new Random<byte[]>
            {
                Data = data,
                CompletionTime = response.Random.CompletionTime
            };

            return new RandomResult<byte[]>(
                random, response.BitsUsed, response.BitsLeft, response.RequestsLeft);
        }

        /// <summary>Generates true random integers within a user-defined range with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random integers to generate. Must be within the [1,10000] range.</param>
        /// <param name="minimum">The lower boundary for the range from which the random numbers will be picked. Must be within the [-1000000000,1000000000] range.</param>
        /// <param name="maximum">The upper boundary for the range from which the random numbers will be picked. Must be within the [-1000000000,1000000000] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1000.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A <see cref="SignedRandomResult{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="minimum" />, or <paramref name="maximum" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<SignedRandomResult<int, IntegerParameters>> GenerateSignedIntegersAsync(
            int count, int minimum, int maximum, bool replacement, string userData, CancellationToken cancellationToken)
        {
            if ((count < 1) || (count > 10000))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, Strings.GetString("random.integer.count.invalid_range"));
            }
            if ((minimum < -1000000000) || (minimum > 1000000000))
            {
                throw new ArgumentOutOfRangeException(nameof(minimum), minimum, Strings.GetString("random.integer.lower_boundary.invalid_range"));
            }
            if ((maximum < -1000000000) || (maximum > 1000000000))
            {
                throw new ArgumentOutOfRangeException(nameof(maximum), maximum, Strings.GetString("random.integer.upper_boundary.invalid_range"));
            }
            if ((userData != null) && (userData.Length > 1000))
            {
                throw new ArgumentException(Strings.GetString("random.user_data.length.invalid_range"), nameof(userData));
            }

            var parameters = new Dictionary<string, object>(6, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey,
                ["n"] = count,
                ["min"] = minimum,
                ["max"] = maximum,
                ["replacement"] = replacement,
                ["base"] = 10,
                ["userData"] = userData
            };

            var response = await InvokeGenerationServiceMethodAsync<RpcSignedRandomResult<RpcIntegersRandom, int>, RpcIntegersRandom, int>(
                "generateSignedIntegers", parameters, cancellationToken).ConfigureAwait(false);

            var random = new SignedRandom<int, IntegerParameters>
            {
                Data = response.Random.Data
            };

            random.Parameters.Minimum = response.Random.Minimum;
            random.Parameters.Maximum = response.Random.Maximum;
            random.Parameters.Replacement = response.Random.Replacement;

            TransferSignedRandomData(response.Random, random);

            return new SignedRandomResult<int, IntegerParameters>(
                random, response.BitsUsed, response.BitsLeft, response.RequestsLeft, response.Signature);
        }

        /// <summary>Generates true random integers within a user-defined range with signature as an asynchronous operation.</summary>
        /// <param name="counts">An array specifying the lengths of the requested sequences. Each value must be within the [1,10000] range and the total sum of all the lengths must be in the [1,10000] range. Up to 10 sequences can be requested.</param>
        /// <param name="minimums">An array specifying the lower boundaries for the requested sequences. Each value must be within the [-1000000000,1000000000] range.</param>
        /// <param name="maximums">An array specifying the upper boundaries for the requested sequences. Each value must be within the [-1000000000,1000000000] range.</param>
        /// <param name="replacements">An array specifying for each requested sequence whether the random numbers in that seqeuence should be picked with replacement.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1000.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A <see cref="RandomResult{T}" /> instance.</returns>
        /// <exception cref="ArgumentException">Counts of argument arrays are different, sequences count is greater than 10, or total count is outside the [1,10000] range.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="counts" />, <paramref name="minimums" />, <paramref name="maximums" />, or <paramref name="replacements" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">One of the values from the arguments <paramref name="counts" />, <paramref name="minimums" />, or <paramref name="maximums" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<SignedRandomResult<int[], IntegerSequenceParameters>> GenerateSignedIntegerSequencesAsync(
            IReadOnlyList<int> counts, IReadOnlyList<int> minimums, IReadOnlyList<int> maximums, IReadOnlyList<bool> replacements, string userData, CancellationToken cancellationToken)
        {
            if (counts == null)
            {
                throw new ArgumentNullException(nameof(counts));
            }
            if (minimums == null)
            {
                throw new ArgumentNullException(nameof(minimums));
            }
            if (maximums == null)
            {
                throw new ArgumentNullException(nameof(maximums));
            }
            if (replacements == null)
            {
                throw new ArgumentNullException(nameof(replacements));
            }

            if ((counts.Count != minimums.Count) ||
                (counts.Count != maximums.Count) ||
                (counts.Count != replacements.Count))
            {
                throw new ArgumentException(Strings.GetString("random.sequence.arguments.different_counts"));
            }
            if (counts.Count > 10)
            {
                throw new ArgumentException(Strings.GetString("random.sequence.count.invalid_range"));
            }

            var count = 0;
            var bases = new int[counts.Count];

            for (var i = 0; i < bases.Length; i++)
            {
                if ((counts[i] < 1) || (counts[i] > 10000))
                {
                    throw new ArgumentOutOfRangeException(nameof(counts) + "." + i, counts[i], Strings.GetString("random.integer.count.invalid_range"));
                }
                if ((minimums[i] < -1000000000) || (minimums[i] > 1000000000))
                {
                    throw new ArgumentOutOfRangeException(nameof(minimums) + "." + i, minimums[i], Strings.GetString("random.integer.lower_boundary.invalid_range"));
                }
                if ((maximums[i] < -1000000000) || (maximums[i] > 1000000000))
                {
                    throw new ArgumentOutOfRangeException(nameof(maximums) + "." + i, maximums[i], Strings.GetString("random.integer.upper_boundary.invalid_range"));
                }

                count += counts[i];
                bases[i] = 10;
            }

            if ((count < 1) || (count > 10000))
            {
                throw new ArgumentException(Strings.GetString("random.sequence.count.invalid_value"));
            }

            var parameters = new Dictionary<string, object>(6, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey,
                ["n"] = counts,
                ["min"] = minimums,
                ["max"] = maximums,
                ["replacement"] = replacements,
                ["base"] = bases,
                ["userData"] = userData
            };

            var response = await InvokeGenerationServiceMethodAsync<RpcSignedRandomResult<RpcIntegerSequencesRandom, int[]>, RpcIntegerSequencesRandom, int[]>(
                "generateSignedIntegerSequences", parameters, cancellationToken).ConfigureAwait(false);

            var random = new SignedRandom<int[], IntegerSequenceParameters>
            {
                Data = response.Random.Data
            };

            random.Parameters.Minimums = response.Random.Minimums;
            random.Parameters.Maximums = response.Random.Maximums;
            random.Parameters.Replacements = response.Random.Replacements;

            TransferSignedRandomData(response.Random, random);

            return new SignedRandomResult<int[], IntegerSequenceParameters>(
                random, response.BitsUsed, response.BitsLeft, response.RequestsLeft, response.Signature);
        }

        /// <summary>Generates true random decimal fractions from a uniform distribution across the [0,1] interval with a user-defined number of decimal places with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random decimal fractions to generate. Must be within the [1,10000] range.</param>
        /// <param name="decimalPlaces">The number of decimal places to use. Must be within the [1,20] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1000.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A <see cref="SignedRandomResult{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="decimalPlaces" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<SignedRandomResult<decimal, DecimalFractionParameters>> GenerateSignedDecimalFractionsAsync(
            int count, int decimalPlaces, bool replacement, string userData, CancellationToken cancellationToken)
        {
            if ((count < 1) || (count > 10000))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, Strings.GetString("random.decimal_fraction.count.invalid_range"));
            }
            if ((decimalPlaces < 1) || (decimalPlaces > 20))
            {
                throw new ArgumentOutOfRangeException(nameof(decimalPlaces), decimalPlaces, Strings.GetString("random.decimal_fraction.decimal_places.invalid_range"));
            }
            if ((userData != null) && (userData.Length > 1000))
            {
                throw new ArgumentException(Strings.GetString("random.user_data.length.invalid_range"), nameof(userData));
            }

            var parameters = new Dictionary<string, object>(5, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey,
                ["n"] = count,
                ["decimalPlaces"] = decimalPlaces,
                ["replacement"] = replacement,
                ["userData"] = userData
            };

            var response = await InvokeGenerationServiceMethodAsync<RpcSignedRandomResult<RpcDecimalFractionsRandom, decimal>, RpcDecimalFractionsRandom, decimal>(
                "generateSignedDecimalFractions", parameters, cancellationToken).ConfigureAwait(false);

            var random = new SignedRandom<decimal, DecimalFractionParameters>
            {
                Data = response.Random.Data
            };

            random.Parameters.DecimalPlaces = response.Random.DecimalPlaces;
            random.Parameters.Replacement = response.Random.Replacement;

            TransferSignedRandomData(response.Random, random);

            return new SignedRandomResult<decimal, DecimalFractionParameters>(
                random, response.BitsUsed, response.BitsLeft, response.RequestsLeft, response.Signature);
        }

        /// <summary>Generates true random numbers from a Gaussian distribution with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random numbers to generate. Must be within the [1,10000] range.</param>
        /// <param name="mean">The distribution's mean. Must be within the [-1000000,1000000] range.</param>
        /// <param name="standardDeviation">The distribution's standard deviation. Must be within the [-1000000,1000000] range.</param>
        /// <param name="significantDigits">The number of significant digits to use. Must be within the [2,20] range.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1000.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A <see cref="SignedRandomResult{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="mean" />, <paramref name="standardDeviation" />, or <paramref name="significantDigits" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<SignedRandomResult<decimal, GaussianParameters>> GenerateSignedGaussiansAsync(
            int count, decimal mean, decimal standardDeviation, int significantDigits, string userData, CancellationToken cancellationToken)
        {
            if ((count < 1) || (count > 10000))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, Strings.GetString("random.gaussian.count.invalid_range"));
            }
            if ((mean < -1000000) || (mean > 1000000))
            {
                throw new ArgumentOutOfRangeException(nameof(mean), mean, Strings.GetString("random.gaussian.mean.invalid_range"));
            }
            if ((standardDeviation < -1000000) || (standardDeviation > 1000000))
            {
                throw new ArgumentOutOfRangeException(nameof(standardDeviation), standardDeviation, Strings.GetString("random.gaussian.standard_deviation.invalid_range"));
            }
            if ((significantDigits < 2) || (significantDigits > 20))
            {
                throw new ArgumentOutOfRangeException(nameof(significantDigits), significantDigits, Strings.GetString("random.gaussian.significant_digits.invalid_range"));
            }
            if ((userData != null) && (userData.Length > 1000))
            {
                throw new ArgumentException(Strings.GetString("random.user_data.length.invalid_range"), nameof(userData));
            }

            var parameters = new Dictionary<string, object>(6, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey,
                ["n"] = count,
                ["mean"] = RandomOrgConvert.DecimalToObject(mean),
                ["standardDeviation"] = RandomOrgConvert.DecimalToObject(standardDeviation),
                ["significantDigits"] = significantDigits,
                ["userData"] = userData
            };

            var response = await InvokeGenerationServiceMethodAsync<RpcSignedRandomResult<RpcGaussiansRandom, decimal>, RpcGaussiansRandom, decimal>(
                "generateSignedGaussians", parameters, cancellationToken).ConfigureAwait(false);

            var random = new SignedRandom<decimal, GaussianParameters>
            {
                Data = response.Random.Data
            };

            random.Parameters.Mean = response.Random.Mean;
            random.Parameters.StandardDeviation = response.Random.StandardDeviation;
            random.Parameters.SignificantDigits = response.Random.SignificantDigits;

            TransferSignedRandomData(response.Random, random);

            return new SignedRandomResult<decimal, GaussianParameters>(
                random, response.BitsUsed, response.BitsLeft, response.RequestsLeft, response.Signature);
        }

        /// <summary>Generates true random strings with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random strings to generate. Must be within the [1,10000] range.</param>
        /// <param name="length">The length of each string. Must be within the [1,20] range.</param>
        /// <param name="characters">A string that contains the set of characters that are allowed to occur in the random strings. The maximum number of characters is 80.</param>
        /// <param name="replacement">Specifies whether the random strings should be picked with replacement.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1000.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A <see cref="SignedRandomResult{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="length" /> is outside the allowable range of values.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="characters" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="characters" /> contains invalid number of characters .</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<SignedRandomResult<string, StringParameters>> GenerateSignedStringsAsync(
            int count, int length, string characters, bool replacement, string userData, CancellationToken cancellationToken)
        {
            if ((count < 1) || (count > 10000))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, Strings.GetString("random.string.count.invalid_range"));
            }
            if ((length < 1) || (length > 20))
            {
                throw new ArgumentOutOfRangeException(nameof(length), length, Strings.GetString("random.string.length.invalid_range"));
            }
            if (characters == null)
            {
                throw new ArgumentNullException(nameof(characters));
            }
            if ((characters.Length < 1) || (characters.Length > 80))
            {
                throw new ArgumentException(Strings.GetString("random.string.characters.length.invalid_range"), nameof(characters));
            }
            if ((userData != null) && (userData.Length > 1000))
            {
                throw new ArgumentException(Strings.GetString("random.user_data.length.invalid_range"), nameof(userData));
            }

            var parameters = new Dictionary<string, object>(6, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey,
                ["n"] = count,
                ["length"] = length,
                ["characters"] = characters,
                ["replacement"] = replacement,
                ["userData"] = userData
            };

            var response = await InvokeGenerationServiceMethodAsync<RpcSignedRandomResult<RpcStringsRandom, string>, RpcStringsRandom, string>(
                "generateSignedStrings", parameters, cancellationToken).ConfigureAwait(false);

            var random = new SignedRandom<string, StringParameters>
            {
                Data = response.Random.Data
            };

            random.Parameters.Length = response.Random.Length;
            random.Parameters.Characters = response.Random.Characters;
            random.Parameters.Replacement = response.Random.Replacement;

            TransferSignedRandomData(response.Random, random);

            return new SignedRandomResult<string, StringParameters>(
                random, response.BitsUsed, response.BitsLeft, response.RequestsLeft, response.Signature);
        }

        /// <summary>Generates version 4 true random UUIDs in accordance with section 4.4 of RFC 4122 with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random UUIDs to generate. Must be within the [1,1000] range.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1000.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A <see cref="SignedRandomResult{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<SignedRandomResult<Guid, UuidParameters>> GenerateSignedUuidsAsync(
            int count, string userData, CancellationToken cancellationToken)
        {
            if ((count < 1) || (count > 1000))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, Strings.GetString("random.uuid.count.invalid_range"));
            }
            if ((userData != null) && (userData.Length > 1000))
            {
                throw new ArgumentException(Strings.GetString("random.user_data.length.invalid_range"), nameof(userData));
            }

            var parameters = new Dictionary<string, object>(3, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey,
                ["n"] = count,
                ["userData"] = userData
            };

            var response = await InvokeGenerationServiceMethodAsync<RpcSignedRandomResult<RpcUuidsRandom, Guid>, RpcUuidsRandom, Guid>(
                "generateSignedUUIDs", parameters, cancellationToken).ConfigureAwait(false);

            var random = new SignedRandom<Guid, UuidParameters>
            {
                Data = response.Random.Data
            };

            TransferSignedRandomData(response.Random, random);

            return new SignedRandomResult<Guid, UuidParameters>(
                random, response.BitsUsed, response.BitsLeft, response.RequestsLeft, response.Signature);
        }

        /// <summary>Generates BLOBs containing true random data with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random blobs to generate. Must be within the [1,100] range.</param>
        /// <param name="size">The size of each blob, measured in bits. Must be within the [1,1048576] range and must be divisible by 8. The total size of all blobs requested must not exceed 1048576 bits.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1000.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A <see cref="SignedRandomResult{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="size" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<SignedRandomResult<byte[], BlobParameters>> GenerateSignedBlobsAsync(
            int count, int size, string userData, CancellationToken cancellationToken)
        {
            if ((count < 1) || (count > 100))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, Strings.GetString("random.blob.count.invalid_range"));
            }
            if ((size < 1) || (size > 1048576))
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, Strings.GetString("random.blob.size.invalid_range"));
            }
            if (size % 8 != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, Strings.GetString("random.blob.size.invalid_division"));
            }
            if (count * size > 1048576)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, Strings.GetString("random.blob.invalid_total_size"));
            }
            if ((userData != null) && (userData.Length > 1000))
            {
                throw new ArgumentException(Strings.GetString("random.user_data.length.invalid_range"), nameof(userData));
            }

            var parameters = new Dictionary<string, object>(4, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey,
                ["n"] = count,
                ["size"] = size,
                ["format"] = "base64",
                ["userData"] = userData
            };

            var response = await InvokeGenerationServiceMethodAsync<RpcSignedRandomResult<RpcBlobsRandom, string>, RpcBlobsRandom, string>(
                "generateSignedBlobs", parameters, cancellationToken).ConfigureAwait(false);

            var data = new byte[response.Random.Data.Count][];

            for (var i = 0; i < data.Length; i++)
            {
                if (response.Random.Data[i] != null)
                {
                    data[i] = Convert.FromBase64String(response.Random.Data[i]);
                }
            }

            var random = new SignedRandom<byte[], BlobParameters>
            {
                Data = data
            };

            random.Parameters.Size = response.Random.Size;

            TransferSignedRandomData(response.Random, random);

            return new SignedRandomResult<byte[], BlobParameters>(
                random, response.BitsUsed, response.BitsLeft, response.RequestsLeft, response.Signature);
        }
    }
}