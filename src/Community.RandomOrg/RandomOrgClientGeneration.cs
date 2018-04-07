using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Community.RandomOrg.Data;
using Community.RandomOrg.Internal;
using Community.RandomOrg.Resources;

namespace Community.RandomOrg
{
    public partial class RandomOrgClient
    {
        /// <summary>Generates true random integers within a user-defined range as an asynchronous operation.</summary>
        /// <param name="count">How many random integers to generate. Must be within the [1,10000] range.</param>
        /// <param name="minimum">The lower boundary for the range from which the random numbers will be picked. Must be within the [-1000000000,1000000000] range.</param>
        /// <param name="maximum">The upper boundary for the range from which the random numbers will be picked. Must be within the [-1000000000,1000000000] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is a <see cref="RandomResult{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="minimum" />, or <paramref name="maximum" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<RandomResult<int>> GenerateIntegersAsync(
            int count, int minimum, int maximum, bool replacement, CancellationToken cancellationToken = default)
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

            cancellationToken.ThrowIfCancellationRequested();

            var parameters = CreateGenerationParameters(5);

            parameters["n"] = count;
            parameters["min"] = minimum;
            parameters["max"] = maximum;
            parameters["replacement"] = replacement;
            parameters["base"] = 10;

            var response = await InvokeGenerationServiceMethodAsync<RpcRandomResult<int>, RpcRandom<int>, int>(
                "generateIntegers", parameters, cancellationToken).ConfigureAwait(false);

            var random = new Random<int>();

            TransferRandom(response.Random, random);

            return new RandomResult<int>(
                random, response.BitsUsed, response.BitsLeft, response.RequestsLeft);
        }

        /// <summary>Generates true random integer sequences within a user-defined ranges as an asynchronous operation.</summary>
        /// <param name="lengths">A collection specifying the lengths of the requested sequences. Each value must be within the [1,10000] range and the total sum of all the lengths must be in the [1,10000] range. The count of sequences must be within the [1,1000] range.</param>
        /// <param name="minimums">A collection specifying the lower boundaries for the requested sequences. Each value must be within the [-1000000000,1000000000] range.</param>
        /// <param name="maximums">A collection specifying the upper boundaries for the requested sequences. Each value must be within the [-1000000000,1000000000] range.</param>
        /// <param name="replacements">A collection specifying for each requested sequence whether the random numbers in that sequence should be picked with replacement.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is a <see cref="RandomResult{T}" /> instance.</returns>
        /// <exception cref="ArgumentException">Counts of argument arrays are different, sequences count is greater than 10, or total count is outside the [1,10000] range.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="lengths" />, <paramref name="minimums" />, <paramref name="maximums" />, or <paramref name="replacements" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">One of the values from the arguments <paramref name="lengths" />, <paramref name="minimums" />, or <paramref name="maximums" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<RandomResult<IReadOnlyList<int>>> GenerateIntegerSequencesAsync(
            IReadOnlyList<int> lengths, IReadOnlyList<int> minimums, IReadOnlyList<int> maximums, IReadOnlyList<bool> replacements, CancellationToken cancellationToken = default)
        {
            if (lengths == null)
            {
                throw new ArgumentNullException(nameof(lengths));
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

            cancellationToken.ThrowIfCancellationRequested();

            var count = lengths.Count;

            if ((count != minimums.Count) ||
                (count != maximums.Count) ||
                (count != replacements.Count))
            {
                throw new ArgumentException(Strings.GetString("random.sequence.arguments.different_size"));
            }
            if ((count < 1) || (count > 1000))
            {
                throw new ArgumentException(Strings.GetString("random.sequence.count.invalid_range"));
            }

            var total = 0;
            var bases = new int[count];

            for (var i = 0; i < count; i++)
            {
                if ((lengths[i] < 1) || (lengths[i] > 10000))
                {
                    throw new ArgumentOutOfRangeException(nameof(lengths) + "." + i, lengths[i], Strings.GetString("random.integer.count.invalid_range"));
                }
                if ((minimums[i] < -1000000000) || (minimums[i] > 1000000000))
                {
                    throw new ArgumentOutOfRangeException(nameof(minimums) + "." + i, minimums[i], Strings.GetString("random.integer.lower_boundary.invalid_range"));
                }
                if ((maximums[i] < -1000000000) || (maximums[i] > 1000000000))
                {
                    throw new ArgumentOutOfRangeException(nameof(maximums) + "." + i, maximums[i], Strings.GetString("random.integer.upper_boundary.invalid_range"));
                }

                total += lengths[i];
                bases[i] = 10;
            }

            if (total > 10000)
            {
                throw new ArgumentException(Strings.GetString("random.sequence.total.invalid_range"));
            }

            var parameters = CreateGenerationParameters(6);

            parameters["n"] = count;
            parameters["length"] = lengths;
            parameters["min"] = minimums;
            parameters["max"] = maximums;
            parameters["replacement"] = replacements;
            parameters["base"] = bases;

            var response = await InvokeGenerationServiceMethodAsync<RpcRandomResult<IReadOnlyList<int>>, RpcRandom<IReadOnlyList<int>>, IReadOnlyList<int>>(
                "generateIntegerSequences", parameters, cancellationToken).ConfigureAwait(false);

            var random = new Random<IReadOnlyList<int>>();

            TransferRandom(response.Random, random);

            return new RandomResult<IReadOnlyList<int>>(
                random, response.BitsUsed, response.BitsLeft, response.RequestsLeft);
        }

        /// <summary>Generates true random decimal fractions from a uniform distribution across the [0,1] interval with a user-defined number of decimal places as an asynchronous operation.</summary>
        /// <param name="count">How many random decimal fractions to generate. Must be within the [1,10000] range.</param>
        /// <param name="decimalPlaces">The number of decimal places to use. Must be within the [1,20] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is a <see cref="RandomResult{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="decimalPlaces" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<RandomResult<decimal>> GenerateDecimalFractionsAsync(
            int count, int decimalPlaces, bool replacement, CancellationToken cancellationToken = default)
        {
            if ((count < 1) || (count > 10000))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, Strings.GetString("random.decimal_fraction.count.invalid_range"));
            }
            if ((decimalPlaces < 1) || (decimalPlaces > 20))
            {
                throw new ArgumentOutOfRangeException(nameof(decimalPlaces), decimalPlaces, Strings.GetString("random.decimal_fraction.decimal_places.invalid_range"));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var parameters = CreateGenerationParameters(3);

            parameters["n"] = count;
            parameters["decimalPlaces"] = decimalPlaces;
            parameters["replacement"] = replacement;

            var response = await InvokeGenerationServiceMethodAsync<RpcRandomResult<decimal>, RpcRandom<decimal>, decimal>(
                "generateDecimalFractions", parameters, cancellationToken).ConfigureAwait(false);

            var random = new Random<decimal>();

            TransferRandom(response.Random, random);

            return new RandomResult<decimal>(
                random, response.BitsUsed, response.BitsLeft, response.RequestsLeft);
        }

        /// <summary>Generates true random numbers from a Gaussian distribution as an asynchronous operation.</summary>
        /// <param name="count">How many random numbers to generate. Must be within the [1,10000] range.</param>
        /// <param name="mean">The distribution's mean. Must be within the [-1000000,1000000] range.</param>
        /// <param name="standardDeviation">The distribution's standard deviation. Must be within the [-1000000,1000000] range.</param>
        /// <param name="significantDigits">The number of significant digits to use. Must be within the [2,20] range.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is a <see cref="RandomResult{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="mean" />, <paramref name="standardDeviation" />, or <paramref name="significantDigits" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<RandomResult<decimal>> GenerateGaussiansAsync(
            int count, decimal mean, decimal standardDeviation, int significantDigits, CancellationToken cancellationToken = default)
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

            cancellationToken.ThrowIfCancellationRequested();

            var parameters = CreateGenerationParameters(4);

            parameters["n"] = count;
            parameters["mean"] = RandomOrgConvert.DecimalToNumber(mean);
            parameters["standardDeviation"] = RandomOrgConvert.DecimalToNumber(standardDeviation);
            parameters["significantDigits"] = significantDigits;

            var response = await InvokeGenerationServiceMethodAsync<RpcRandomResult<decimal>, RpcRandom<decimal>, decimal>(
                "generateGaussians", parameters, cancellationToken).ConfigureAwait(false);

            var random = new Random<decimal>();

            TransferRandom(response.Random, random);

            return new RandomResult<decimal>(
                random, response.BitsUsed, response.BitsLeft, response.RequestsLeft);
        }

        /// <summary>Generates true random strings as an asynchronous operation.</summary>
        /// <param name="count">How many random strings to generate. Must be within the [1,10000] range.</param>
        /// <param name="length">The length of each string. Must be within the [1,20] range.</param>
        /// <param name="characters">A string that contains the set of characters that are allowed to occur in the random strings. The maximum number of characters is 80.</param>
        /// <param name="replacement">Specifies whether the random strings should be picked with replacement.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is a <see cref="RandomResult{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="length" /> is outside the allowable range of values.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="characters" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="characters" /> contains invalid number of characters .</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<RandomResult<string>> GenerateStringsAsync(
            int count, int length, string characters, bool replacement, CancellationToken cancellationToken = default)
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

            cancellationToken.ThrowIfCancellationRequested();

            var parameters = CreateGenerationParameters(4);

            parameters["n"] = count;
            parameters["length"] = length;
            parameters["characters"] = characters;
            parameters["replacement"] = replacement;

            var response = await InvokeGenerationServiceMethodAsync<RpcRandomResult<string>, RpcRandom<string>, string>(
                "generateStrings", parameters, cancellationToken).ConfigureAwait(false);

            var random = new Random<string>();

            TransferRandom(response.Random, random);

            return new RandomResult<string>(
                random, response.BitsUsed, response.BitsLeft, response.RequestsLeft);
        }

        /// <summary>Generates version 4 true random UUIDs in accordance with section 4.4 of RFC 4122 as an asynchronous operation.</summary>
        /// <param name="count">How many random UUIDs to generate. Must be within the [1,1000] range.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is a <see cref="RandomResult{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<RandomResult<Guid>> GenerateUuidsAsync(
            int count, CancellationToken cancellationToken = default)
        {
            if ((count < 1) || (count > 1000))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, Strings.GetString("random.uuid.count.invalid_range"));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var parameters = CreateGenerationParameters(1);

            parameters["n"] = count;

            var response = await InvokeGenerationServiceMethodAsync<RpcRandomResult<Guid>, RpcRandom<Guid>, Guid>(
                "generateUUIDs", parameters, cancellationToken).ConfigureAwait(false);

            var random = new Random<Guid>();

            TransferRandom(response.Random, random);

            return new RandomResult<Guid>(
                random, response.BitsUsed, response.BitsLeft, response.RequestsLeft);
        }

        /// <summary>Generates BLOBs containing true random data as an asynchronous operation.</summary>
        /// <param name="count">How many random blobs to generate. Must be within the [1,100] range.</param>
        /// <param name="size">The size of each blob, measured in bytes. Must be within the [1,131072] range. The total size of all blobs requested must not exceed 131072 bytes.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is a <see cref="RandomResult{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="size" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<RandomResult<byte[]>> GenerateBlobsAsync(
            int count, int size, CancellationToken cancellationToken = default)
        {
            if ((count < 1) || (count > 100))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, Strings.GetString("random.blob.count.invalid_range"));
            }
            if ((size < 1) || (size > 131072))
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, Strings.GetString("random.blob.size.invalid_range"));
            }
            if (count * size > 131072)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, Strings.GetString("random.blob.invalid_total_size"));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var parameters = CreateGenerationParameters(3);

            parameters["n"] = count;
            parameters["size"] = size * 8;
            parameters["format"] = "base64";

            var response = await InvokeGenerationServiceMethodAsync<RpcRandomResult<byte[]>, RpcRandom<byte[]>, byte[]>(
                "generateBlobs", parameters, cancellationToken).ConfigureAwait(false);

            var random = new Random<byte[]>();

            TransferRandom(response.Random, random);

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
        /// <returns>A task that represents the asynchronous operation. The task result is a <see cref="SignedRandomResult{TValue, TParameters}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="minimum" />, or <paramref name="maximum" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<SignedRandomResult<int, IntegerParameters>> GenerateSignedIntegersAsync(
            int count, int minimum, int maximum, bool replacement, string userData = null, CancellationToken cancellationToken = default)
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

            cancellationToken.ThrowIfCancellationRequested();

            var parameters = CreateGenerationParameters(6);

            parameters["n"] = count;
            parameters["min"] = minimum;
            parameters["max"] = maximum;
            parameters["replacement"] = replacement;
            parameters["base"] = 10;
            parameters["userData"] = userData;

            var response = await InvokeGenerationServiceMethodAsync<RpcSignedRandomResult<RpcIntegersRandom, int>, RpcIntegersRandom, int>(
                "generateSignedIntegers", parameters, cancellationToken).ConfigureAwait(false);

            var random = new SignedRandom<int, IntegerParameters>();

            TransferRandom(response.Random, random);

            random.Parameters.Minimum = response.Random.Minimum;
            random.Parameters.Maximum = response.Random.Maximum;
            random.Parameters.Replacement = response.Random.Replacement;

            return new SignedRandomResult<int, IntegerParameters>(
                random, response.BitsUsed, response.BitsLeft, response.RequestsLeft, response.Signature);
        }

        /// <summary>Generates true random integer sequences within a user-defined ranges with signature as an asynchronous operation.</summary>
        /// <param name="lengths">A collection specifying the lengths of the requested sequences. Each value must be within the [1,10000] range and the total sum of all the lengths must be in the [1,10000] range. The count of sequences must be within the [1,1000] range.</param>
        /// <param name="minimums">A collection specifying the lower boundaries for the requested sequences. Each value must be within the [-1000000000,1000000000] range.</param>
        /// <param name="maximums">A collection specifying the upper boundaries for the requested sequences. Each value must be within the [-1000000000,1000000000] range.</param>
        /// <param name="replacements">A collection specifying for each requested sequence whether the random numbers in that sequence should be picked with replacement.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1000.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is a <see cref="RandomResult{T}" /> instance.</returns>
        /// <exception cref="ArgumentException">Counts of argument arrays are different, sequences count is greater than 10, or total count is outside the [1,10000] range.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="lengths" />, <paramref name="minimums" />, <paramref name="maximums" />, or <paramref name="replacements" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">One of the values from the arguments <paramref name="lengths" />, <paramref name="minimums" />, or <paramref name="maximums" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<SignedRandomResult<IReadOnlyList<int>, IntegerSequenceParameters>> GenerateSignedIntegerSequencesAsync(
             IReadOnlyList<int> lengths, IReadOnlyList<int> minimums, IReadOnlyList<int> maximums, IReadOnlyList<bool> replacements, string userData = null, CancellationToken cancellationToken = default)
        {
            if (lengths == null)
            {
                throw new ArgumentNullException(nameof(lengths));
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

            cancellationToken.ThrowIfCancellationRequested();

            var count = lengths.Count;

            if ((count != minimums.Count) ||
                (count != maximums.Count) ||
                (count != replacements.Count))
            {
                throw new ArgumentException(Strings.GetString("random.sequence.arguments.different_size"));
            }
            if ((count < 1) || (count > 1000))
            {
                throw new ArgumentException(Strings.GetString("random.sequence.count.invalid_range"));
            }

            var total = 0;
            var bases = new int[count];

            for (var i = 0; i < count; i++)
            {
                if ((lengths[i] < 1) || (lengths[i] > 10000))
                {
                    throw new ArgumentOutOfRangeException(nameof(lengths) + "." + i, lengths[i], Strings.GetString("random.integer.count.invalid_range"));
                }
                if ((minimums[i] < -1000000000) || (minimums[i] > 1000000000))
                {
                    throw new ArgumentOutOfRangeException(nameof(minimums) + "." + i, minimums[i], Strings.GetString("random.integer.lower_boundary.invalid_range"));
                }
                if ((maximums[i] < -1000000000) || (maximums[i] > 1000000000))
                {
                    throw new ArgumentOutOfRangeException(nameof(maximums) + "." + i, maximums[i], Strings.GetString("random.integer.upper_boundary.invalid_range"));
                }

                total += lengths[i];
                bases[i] = 10;
            }

            if (total > 10000)
            {
                throw new ArgumentException(Strings.GetString("random.sequence.total.invalid_range"));
            }
            if ((userData != null) && (userData.Length > 1000))
            {
                throw new ArgumentException(Strings.GetString("random.user_data.length.invalid_range"), nameof(userData));
            }

            var parameters = CreateGenerationParameters(7);

            parameters["n"] = count;
            parameters["length"] = lengths;
            parameters["min"] = minimums;
            parameters["max"] = maximums;
            parameters["replacement"] = replacements;
            parameters["base"] = bases;
            parameters["userData"] = userData;

            var response = await InvokeGenerationServiceMethodAsync<RpcSignedRandomResult<RpcIntegerSequencesRandom, IReadOnlyList<int>>, RpcIntegerSequencesRandom, IReadOnlyList<int>>(
                "generateSignedIntegerSequences", parameters, cancellationToken).ConfigureAwait(false);

            var random = new SignedRandom<IReadOnlyList<int>, IntegerSequenceParameters>();

            TransferRandom(response.Random, random);

            random.Parameters.Minimums = response.Random.Minimums;
            random.Parameters.Maximums = response.Random.Maximums;
            random.Parameters.Replacements = response.Random.Replacements;

            return new SignedRandomResult<IReadOnlyList<int>, IntegerSequenceParameters>(
                random, response.BitsUsed, response.BitsLeft, response.RequestsLeft, response.Signature);
        }

        /// <summary>Generates true random decimal fractions from a uniform distribution across the [0,1] interval with a user-defined number of decimal places with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random decimal fractions to generate. Must be within the [1,10000] range.</param>
        /// <param name="decimalPlaces">The number of decimal places to use. Must be within the [1,20] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1000.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is a <see cref="SignedRandomResult{TValue, TParameters}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="decimalPlaces" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<SignedRandomResult<decimal, DecimalFractionParameters>> GenerateSignedDecimalFractionsAsync(
            int count, int decimalPlaces, bool replacement, string userData = null, CancellationToken cancellationToken = default)
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

            cancellationToken.ThrowIfCancellationRequested();

            var parameters = CreateGenerationParameters(4);

            parameters["n"] = count;
            parameters["decimalPlaces"] = decimalPlaces;
            parameters["replacement"] = replacement;
            parameters["userData"] = userData;

            var response = await InvokeGenerationServiceMethodAsync<RpcSignedRandomResult<RpcDecimalFractionsRandom, decimal>, RpcDecimalFractionsRandom, decimal>(
                "generateSignedDecimalFractions", parameters, cancellationToken).ConfigureAwait(false);

            var random = new SignedRandom<decimal, DecimalFractionParameters>();

            TransferRandom(response.Random, random);

            random.Parameters.DecimalPlaces = response.Random.DecimalPlaces;
            random.Parameters.Replacement = response.Random.Replacement;

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
        /// <returns>A task that represents the asynchronous operation. The task result is a <see cref="SignedRandomResult{TValue, TParameters}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="mean" />, <paramref name="standardDeviation" />, or <paramref name="significantDigits" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<SignedRandomResult<decimal, GaussianParameters>> GenerateSignedGaussiansAsync(
            int count, decimal mean, decimal standardDeviation, int significantDigits, string userData = null, CancellationToken cancellationToken = default)
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

            cancellationToken.ThrowIfCancellationRequested();

            var parameters = CreateGenerationParameters(5);

            parameters["n"] = count;
            parameters["mean"] = RandomOrgConvert.DecimalToNumber(mean);
            parameters["standardDeviation"] = RandomOrgConvert.DecimalToNumber(standardDeviation);
            parameters["significantDigits"] = significantDigits;
            parameters["userData"] = userData;

            var response = await InvokeGenerationServiceMethodAsync<RpcSignedRandomResult<RpcGaussiansRandom, decimal>, RpcGaussiansRandom, decimal>(
                "generateSignedGaussians", parameters, cancellationToken).ConfigureAwait(false);

            var random = new SignedRandom<decimal, GaussianParameters>();

            TransferRandom(response.Random, random);

            random.Parameters.Mean = response.Random.Mean;
            random.Parameters.StandardDeviation = response.Random.StandardDeviation;
            random.Parameters.SignificantDigits = response.Random.SignificantDigits;

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
        /// <returns>A task that represents the asynchronous operation. The task result is a <see cref="SignedRandomResult{TValue, TParameters}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="length" /> is outside the allowable range of values.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="characters" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="characters" /> contains invalid number of characters .</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<SignedRandomResult<string, StringParameters>> GenerateSignedStringsAsync(
            int count, int length, string characters, bool replacement, string userData = null, CancellationToken cancellationToken = default)
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

            cancellationToken.ThrowIfCancellationRequested();

            var parameters = CreateGenerationParameters(5);

            parameters["n"] = count;
            parameters["length"] = length;
            parameters["characters"] = characters;
            parameters["replacement"] = replacement;
            parameters["userData"] = userData;

            var response = await InvokeGenerationServiceMethodAsync<RpcSignedRandomResult<RpcStringsRandom, string>, RpcStringsRandom, string>(
                "generateSignedStrings", parameters, cancellationToken).ConfigureAwait(false);

            var random = new SignedRandom<string, StringParameters>();

            TransferRandom(response.Random, random);

            random.Parameters.Length = response.Random.Length;
            random.Parameters.Characters = response.Random.Characters;
            random.Parameters.Replacement = response.Random.Replacement;

            return new SignedRandomResult<string, StringParameters>(
                random, response.BitsUsed, response.BitsLeft, response.RequestsLeft, response.Signature);
        }

        /// <summary>Generates version 4 true random UUIDs in accordance with section 4.4 of RFC 4122 with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random UUIDs to generate. Must be within the [1,1000] range.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1000.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is a <see cref="SignedRandomResult{TValue, TParameters}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<SignedRandomResult<Guid, UuidParameters>> GenerateSignedUuidsAsync(
            int count, string userData = null, CancellationToken cancellationToken = default)
        {
            if ((count < 1) || (count > 1000))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, Strings.GetString("random.uuid.count.invalid_range"));
            }
            if ((userData != null) && (userData.Length > 1000))
            {
                throw new ArgumentException(Strings.GetString("random.user_data.length.invalid_range"), nameof(userData));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var parameters = CreateGenerationParameters(2);

            parameters["n"] = count;
            parameters["userData"] = userData;

            var response = await InvokeGenerationServiceMethodAsync<RpcSignedRandomResult<RpcUuidsRandom, Guid>, RpcUuidsRandom, Guid>(
                "generateSignedUUIDs", parameters, cancellationToken).ConfigureAwait(false);

            var random = new SignedRandom<Guid, UuidParameters>();

            TransferRandom(response.Random, random);

            return new SignedRandomResult<Guid, UuidParameters>(
                random, response.BitsUsed, response.BitsLeft, response.RequestsLeft, response.Signature);
        }

        /// <summary>Generates BLOBs containing true random data with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random blobs to generate. Must be within the [1,100] range.</param>
        /// <param name="size">The size of each blob, measured in bytes. Must be within the [1,131072] range. The total size of all blobs requested must not exceed 131072 bytes.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1000.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is a <see cref="SignedRandomResult{TValue, TParameters}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="size" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<SignedRandomResult<byte[], BlobParameters>> GenerateSignedBlobsAsync(
            int count, int size, string userData = null, CancellationToken cancellationToken = default)
        {
            if ((count < 1) || (count > 100))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, Strings.GetString("random.blob.count.invalid_range"));
            }
            if ((size < 1) || (size > 131072))
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, Strings.GetString("random.blob.size.invalid_range"));
            }
            if (count * size > 131072)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, Strings.GetString("random.blob.invalid_total_size"));
            }
            if ((userData != null) && (userData.Length > 1000))
            {
                throw new ArgumentException(Strings.GetString("random.user_data.length.invalid_range"), nameof(userData));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var parameters = CreateGenerationParameters(4);

            parameters["n"] = count;
            parameters["size"] = size * 8;
            parameters["format"] = "base64";
            parameters["userData"] = userData;

            var response = await InvokeGenerationServiceMethodAsync<RpcSignedRandomResult<RpcBlobsRandom, byte[]>, RpcBlobsRandom, byte[]>(
                "generateSignedBlobs", parameters, cancellationToken).ConfigureAwait(false);

            var random = new SignedRandom<byte[], BlobParameters>();

            TransferRandom(response.Random, random);

            random.Parameters.Size = response.Random.Size / 8;

            return new SignedRandomResult<byte[], BlobParameters>(
                random, response.BitsUsed, response.BitsLeft, response.RequestsLeft, response.Signature);
        }
    }
}