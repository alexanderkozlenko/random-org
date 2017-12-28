using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Community.RandomOrg.Data;
using Community.RandomOrg.Resources;

namespace Community.RandomOrg
{
    partial class RandomOrgClient
    {
        /// <summary>Generates true random integers within a user-defined range as an asynchronous operation.</summary>
        /// <param name="count">How many random integers to generate. Must be within the [1,1e4] range.</param>
        /// <param name="minimum">The lower boundary for the range from which the random numbers will be picked. Must be within the [-1e9,1e9] range.</param>
        /// <param name="maximum">The upper boundary for the range from which the random numbers will be picked. Must be within the [-1e9,1e9] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="minimum" />, or <paramref name="maximum" /> is outside the allowable range of values.</exception>
        /// <exception cref="InvalidOperationException">The API key is not specified.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An HTTP error occurred during service method invocation.</exception>
        public async Task<SimpleGenerationInfo<int>> GenerateIntegersAsync(
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
            if (_apiKey == null)
            {
                throw new InvalidOperationException(Strings.GetString("client.api_key.required"));
            }

            var @params = new Dictionary<string, object>(5, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey,
                ["n"] = count,
                ["min"] = minimum,
                ["max"] = maximum
            };

            if (!replacement)
            {
                @params["replacement"] = replacement;
            }

            var result = await InvokeRandomOrgMethod<RpcSimpleRandomResult<int>, RpcSimpleRandom<int>, int>(
                "generateIntegers", @params, cancellationToken).ConfigureAwait(false);

            var random = new SimpleRandom<int>();

            TransferValues(result.Random, random);

            return new SimpleGenerationInfo<int>(
                random, result.BitsUsed, result.BitsLeft, result.RequestsLeft);
        }

        /// <summary>Generates true random decimal fractions from a uniform distribution across the [0,1] interval with a user-defined number of decimal places as an asynchronous operation.</summary>
        /// <param name="count">How many random decimal fractions to generate. Must be within the [1,1e4] range.</param>
        /// <param name="decimalPlaces">The number of decimal places to use. Must be within the [1,20] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="decimalPlaces" /> is outside the allowable range of values.</exception>
        /// <exception cref="InvalidOperationException">The API key is not specified.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An HTTP error occurred during service method invocation.</exception>
        public async Task<SimpleGenerationInfo<decimal>> GenerateDecimalFractionsAsync(
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
            if (_apiKey == null)
            {
                throw new InvalidOperationException(Strings.GetString("client.api_key.required"));
            }

            var @params = new Dictionary<string, object>(4, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey,
                ["n"] = count,
                ["decimalPlaces"] = decimalPlaces
            };

            if (!replacement)
            {
                @params["replacement"] = replacement;
            }

            var result = await InvokeRandomOrgMethod<RpcSimpleRandomResult<decimal>, RpcSimpleRandom<decimal>, decimal>(
                "generateDecimalFractions", @params, cancellationToken).ConfigureAwait(false);

            var random = new SimpleRandom<decimal>();

            TransferValues(result.Random, random);

            return new SimpleGenerationInfo<decimal>(
                random, result.BitsUsed, result.BitsLeft, result.RequestsLeft);
        }

        /// <summary>Generates true random numbers from a Gaussian distribution as an asynchronous operation.</summary>
        /// <param name="count">How many random numbers to generate. Must be within the [1,1e4] range.</param>
        /// <param name="mean">The distribution's mean. Must be within the [-1e6,1e6] range.</param>
        /// <param name="standardDeviation">The distribution's standard deviation. Must be within the [-1e6,1e6] range.</param>
        /// <param name="significantDigits">The number of significant digits to use. Must be within the [2,20] range.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="mean" />, <paramref name="standardDeviation" />, or <paramref name="significantDigits" /> is outside the allowable range of values.</exception>
        /// <exception cref="InvalidOperationException">The API key is not specified.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An HTTP error occurred during service method invocation.</exception>
        public async Task<SimpleGenerationInfo<decimal>> GenerateGaussiansAsync(
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
            if (_apiKey == null)
            {
                throw new InvalidOperationException(Strings.GetString("client.api_key.required"));
            }

            var @params = new Dictionary<string, object>(5, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey,
                ["n"] = count,
                ["mean"] = RandomOrgConvert.ToObject(mean),
                ["standardDeviation"] = RandomOrgConvert.ToObject(standardDeviation),
                ["significantDigits"] = significantDigits
            };

            var result = await InvokeRandomOrgMethod<RpcSimpleRandomResult<decimal>, RpcSimpleRandom<decimal>, decimal>(
                "generateGaussians", @params, cancellationToken).ConfigureAwait(false);

            var random = new SimpleRandom<decimal>();

            TransferValues(result.Random, random);

            return new SimpleGenerationInfo<decimal>(
                random, result.BitsUsed, result.BitsLeft, result.RequestsLeft);
        }

        /// <summary>Generates true random strings as an asynchronous operation.</summary>
        /// <param name="count">How many random strings to generate. Must be within the [1,1e4] range.</param>
        /// <param name="length">The length of each string. Must be within the [1,20] range.</param>
        /// <param name="characters">A string that contains the set of characters that are allowed to occur in the random strings. The maximum number of characters is 80.</param>
        /// <param name="replacement">Specifies whether the random strings should be picked with replacement.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="length" /> is outside the allowable range of values.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="characters" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="characters" /> contains invalid number of characters .</exception>
        /// <exception cref="InvalidOperationException">The API key is not specified.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An HTTP error occurred during service method invocation.</exception>
        public async Task<SimpleGenerationInfo<string>> GenerateStringsAsync(
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
            if (_apiKey == null)
            {
                throw new InvalidOperationException(Strings.GetString("client.api_key.required"));
            }

            var @params = new Dictionary<string, object>(5, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey,
                ["n"] = count,
                ["length"] = length,
                ["characters"] = characters
            };

            if (!replacement)
            {
                @params["replacement"] = replacement;
            }

            var result = await InvokeRandomOrgMethod<RpcSimpleRandomResult<string>, RpcSimpleRandom<string>, string>(
                "generateStrings", @params, cancellationToken).ConfigureAwait(false);

            var random = new SimpleRandom<string>();

            TransferValues(result.Random, random);

            return new SimpleGenerationInfo<string>(
                random, result.BitsUsed, result.BitsLeft, result.RequestsLeft);
        }

        /// <summary>Generates version 4 true random UUIDs in accordance with section 4.4 of RFC 4122 as an asynchronous operation.</summary>
        /// <param name="count">How many random UUIDs to generate. Must be within the [1,1e3] range.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> is outside the allowable range of values.</exception>
        /// <exception cref="InvalidOperationException">The API key is not specified.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An HTTP error occurred during service method invocation.</exception>
        public async Task<SimpleGenerationInfo<Guid>> GenerateUuidsAsync(
            int count, CancellationToken cancellationToken)
        {
            if ((count < 1) || (count > 1000))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, Strings.GetString("random.uuid.count.invalid_range"));
            }
            if (_apiKey == null)
            {
                throw new InvalidOperationException(Strings.GetString("client.api_key.required"));
            }

            var @params = new Dictionary<string, object>(2, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey,
                ["n"] = count,
            };

            var result = await InvokeRandomOrgMethod<RpcSimpleRandomResult<Guid>, RpcSimpleRandom<Guid>, Guid>(
                "generateUUIDs", @params, cancellationToken).ConfigureAwait(false);

            var random = new SimpleRandom<Guid>();

            TransferValues(result.Random, random);

            return new SimpleGenerationInfo<Guid>(
                random, result.BitsUsed, result.BitsLeft, result.RequestsLeft);
        }

        /// <summary>Generates BLOBs containing true random data as an asynchronous operation.</summary>
        /// <param name="count">How many random blobs to generate. Must be within the [1,100] range.</param>
        /// <param name="size">The size of each blob, measured in bits. Must be within the [1,1048576] range and must be divisible by 8. The total size of all blobs requested must not exceed 1048576 bits.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="size" /> is outside the allowable range of values.</exception>
        /// <exception cref="InvalidOperationException">The API key is not specified.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An HTTP error occurred during service method invocation.</exception>
        public async Task<SimpleGenerationInfo<byte[]>> GenerateBlobsAsync(
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
            if (_apiKey == null)
            {
                throw new InvalidOperationException(Strings.GetString("client.api_key.required"));
            }

            var @params = new Dictionary<string, object>(3, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey,
                ["n"] = count,
                ["size"] = size
            };

            var result = await InvokeRandomOrgMethod<RpcSimpleRandomResult<string>, RpcSimpleRandom<string>, string>(
                "generateBlobs", @params, cancellationToken).ConfigureAwait(false);

            var random = new SimpleRandom<byte[]>
            {
                CompletionTime = result.Random.CompletionTime
            };

            var data = new byte[result.Random.Data.Count][];

            for (var i = 0; i < data.Length; i++)
            {
                data[i] = Convert.FromBase64String(result.Random.Data[i]);
            }

            random.Data = data;

            return new SimpleGenerationInfo<byte[]>(
                random, result.BitsUsed, result.BitsLeft, result.RequestsLeft);
        }

        /// <summary>Generates true random integers within a user-defined range with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random integers to generate. Must be within the [1,1e4] range.</param>
        /// <param name="minimum">The lower boundary for the range from which the random numbers will be picked. Must be within the [-1e9,1e9] range.</param>
        /// <param name="maximum">The upper boundary for the range from which the random numbers will be picked. Must be within the [-1e9,1e9] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1e3.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="minimum" />, or <paramref name="maximum" /> is outside the allowable range of values.</exception>
        /// <exception cref="InvalidOperationException">The API key is not specified.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An HTTP error occurred during service method invocation.</exception>
        public async Task<SignedGenerationInfo<SignedIntegersRandom, int>> GenerateSignedIntegersAsync(
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
            if (_apiKey == null)
            {
                throw new InvalidOperationException(Strings.GetString("client.api_key.required"));
            }

            var @params = new Dictionary<string, object>(6, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey,
                ["n"] = count,
                ["min"] = minimum,
                ["max"] = maximum
            };

            if (!replacement)
            {
                @params["replacement"] = replacement;
            }
            if (userData != null)
            {
                @params["userData"] = userData;
            }

            var result = await InvokeRandomOrgMethod<RpcSignedRandomResult<RpcSignedIntegersRandom, int>, RpcSignedIntegersRandom, int>(
                "generateSignedIntegers", @params, cancellationToken).ConfigureAwait(false);

            var random = new SignedIntegersRandom
            {
                Minimum = (int)result.Random.Minimum,
                Maximum = (int)result.Random.Maximum,
                Replacement = result.Random.Replacement
            };

            TransferValues(result.Random, random);

            return new SignedGenerationInfo<SignedIntegersRandom, int>(
                random, result.BitsUsed, result.BitsLeft, result.RequestsLeft, result.Signature);
        }

        /// <summary>Generates true random decimal fractions from a uniform distribution across the [0,1] interval with a user-defined number of decimal places with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random decimal fractions to generate. Must be within the [1,1e4] range.</param>
        /// <param name="decimalPlaces">The number of decimal places to use. Must be within the [1,20] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1e3.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="decimalPlaces" /> is outside the allowable range of values.</exception>
        /// <exception cref="InvalidOperationException">The API key is not specified.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An HTTP error occurred during service method invocation.</exception>
        public async Task<SignedGenerationInfo<SignedDecimalFractionsRandom, decimal>> GenerateSignedDecimalFractionsAsync(
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
            if (_apiKey == null)
            {
                throw new InvalidOperationException(Strings.GetString("client.api_key.required"));
            }

            var @params = new Dictionary<string, object>(5, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey,
                ["n"] = count,
                ["decimalPlaces"] = decimalPlaces
            };

            if (!replacement)
            {
                @params["replacement"] = replacement;
            }
            if (userData != null)
            {
                @params["userData"] = userData;
            }

            var result = await InvokeRandomOrgMethod<RpcSignedRandomResult<RpcSignedDecimalFractionsRandom, decimal>, RpcSignedDecimalFractionsRandom, decimal>(
                "generateSignedDecimalFractions", @params, cancellationToken).ConfigureAwait(false);

            var random = new SignedDecimalFractionsRandom
            {
                DecimalPlaces = (int)result.Random.DecimalPlaces,
                Replacement = result.Random.Replacement
            };

            TransferValues(result.Random, random);

            return new SignedGenerationInfo<SignedDecimalFractionsRandom, decimal>(
                random, result.BitsUsed, result.BitsLeft, result.RequestsLeft, result.Signature);
        }

        /// <summary>Generates true random numbers from a Gaussian distribution with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random numbers to generate. Must be within the [1,1e4] range.</param>
        /// <param name="mean">The distribution's mean. Must be within the [-1e6,1e6] range.</param>
        /// <param name="standardDeviation">The distribution's standard deviation. Must be within the [-1e6,1e6] range.</param>
        /// <param name="significantDigits">The number of significant digits to use. Must be within the [2,20] range.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1e3.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="mean" />, <paramref name="standardDeviation" />, or <paramref name="significantDigits" /> is outside the allowable range of values.</exception>
        /// <exception cref="InvalidOperationException">The API key is not specified.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An HTTP error occurred during service method invocation.</exception>
        public async Task<SignedGenerationInfo<SignedGaussiansRandom, decimal>> GenerateSignedGaussiansAsync(
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
            if (_apiKey == null)
            {
                throw new InvalidOperationException(Strings.GetString("client.api_key.required"));
            }

            var @params = new Dictionary<string, object>(6, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey,
                ["n"] = count,
                ["mean"] = RandomOrgConvert.ToObject(mean),
                ["standardDeviation"] = RandomOrgConvert.ToObject(standardDeviation),
                ["significantDigits"] = significantDigits
            };

            if (userData != null)
            {
                @params["userData"] = userData;
            }

            var result = await InvokeRandomOrgMethod<RpcSignedRandomResult<RpcSignedGaussiansRandom, decimal>, RpcSignedGaussiansRandom, decimal>(
                "generateSignedGaussians", @params, cancellationToken).ConfigureAwait(false);

            var random = new SignedGaussiansRandom
            {
                Mean = result.Random.Mean,
                StandardDeviation = result.Random.StandardDeviation,
                SignificantDigits = (int)result.Random.SignificantDigits
            };

            TransferValues(result.Random, random);

            return new SignedGenerationInfo<SignedGaussiansRandom, decimal>(
                random, result.BitsUsed, result.BitsLeft, result.RequestsLeft, result.Signature);
        }

        /// <summary>Generates true random strings with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random strings to generate. Must be within the [1,1e4] range.</param>
        /// <param name="length">The length of each string. Must be within the [1,20] range.</param>
        /// <param name="characters">A string that contains the set of characters that are allowed to occur in the random strings. The maximum number of characters is 80.</param>
        /// <param name="replacement">Specifies whether the random strings should be picked with replacement.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1e3.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="length" /> is outside the allowable range of values.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="characters" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="characters" /> contains invalid number of characters .</exception>
        /// <exception cref="InvalidOperationException">The API key is not specified.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An HTTP error occurred during service method invocation.</exception>
        public async Task<SignedGenerationInfo<SignedStringsRandom, string>> GenerateSignedStringsAsync(
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
            if (_apiKey == null)
            {
                throw new InvalidOperationException(Strings.GetString("client.api_key.required"));
            }

            var @params = new Dictionary<string, object>(6, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey,
                ["n"] = count,
                ["length"] = length,
                ["characters"] = characters
            };

            if (!replacement)
            {
                @params["replacement"] = replacement;
            }
            if (userData != null)
            {
                @params["userData"] = userData;
            }

            var result = await InvokeRandomOrgMethod<RpcSignedRandomResult<RpcSignedStringsRandom, string>, RpcSignedStringsRandom, string>(
                "generateSignedStrings", @params, cancellationToken).ConfigureAwait(false);

            var random = new SignedStringsRandom
            {
                Length = (int)result.Random.Length,
                Characters = result.Random.Characters,
                Replacement = result.Random.Replacement
            };

            TransferValues(result.Random, random);

            return new SignedGenerationInfo<SignedStringsRandom, string>(
                random, result.BitsUsed, result.BitsLeft, result.RequestsLeft, result.Signature);
        }

        /// <summary>Generates version 4 true random UUIDs in accordance with section 4.4 of RFC 4122 with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random UUIDs to generate. Must be within the [1,1e3] range.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1e3.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> is outside the allowable range of values.</exception>
        /// <exception cref="InvalidOperationException">The API key is not specified.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An HTTP error occurred during service method invocation.</exception>
        public async Task<SignedGenerationInfo<SignedUuidsRandom, Guid>> GenerateSignedUuidsAsync(
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
            if (_apiKey == null)
            {
                throw new InvalidOperationException(Strings.GetString("client.api_key.required"));
            }

            var @params = new Dictionary<string, object>(3, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey,
                ["n"] = count,
            };

            if (userData != null)
            {
                @params["userData"] = userData;
            }

            var result = await InvokeRandomOrgMethod<RpcSignedRandomResult<RpcSignedUuidsRandom, Guid>, RpcSignedUuidsRandom, Guid>(
                "generateSignedUUIDs", @params, cancellationToken).ConfigureAwait(false);

            var random = new SignedUuidsRandom();

            TransferValues(result.Random, random);

            return new SignedGenerationInfo<SignedUuidsRandom, Guid>(
                random, result.BitsUsed, result.BitsLeft, result.RequestsLeft, result.Signature);
        }

        /// <summary>Generates BLOBs containing true random data with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random blobs to generate. Must be within the [1,100] range.</param>
        /// <param name="size">The size of each blob, measured in bits. Must be within the [1,1048576] range and must be divisible by 8. The total size of all blobs requested must not exceed 1048576 bits.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1e3.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="size" /> is outside the allowable range of values.</exception>
        /// <exception cref="InvalidOperationException">The API key is not specified.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An HTTP error occurred during service method invocation.</exception>
        public async Task<SignedGenerationInfo<SignedBlobsRandom, byte[]>> GenerateSignedBlobsAsync(
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
            if (_apiKey == null)
            {
                throw new InvalidOperationException(Strings.GetString("client.api_key.required"));
            }

            var @params = new Dictionary<string, object>(4, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey,
                ["n"] = count,
                ["size"] = size
            };

            if (userData != null)
            {
                @params["userData"] = userData;
            }

            var result = await InvokeRandomOrgMethod<RpcSignedRandomResult<RpcSignedBlobsRandom, string>, RpcSignedBlobsRandom, string>(
                "generateSignedBlobs", @params, cancellationToken).ConfigureAwait(false);

            var random = new SignedBlobsRandom
            {
                ApiKeyHash = result.Random.ApiKeyHash,
                CompletionTime = result.Random.CompletionTime,
                SerialNumber = result.Random.SerialNumber,
                Size = (int)result.Random.Size
            };

            var data = new byte[result.Random.Data.Count][];

            for (var i = 0; i < data.Length; i++)
            {
                data[i] = Convert.FromBase64String(result.Random.Data[i]);
            }

            random.Data = data;
            random.License.Type = result.Random.License.Type;
            random.License.Text = result.Random.License.Text;
            random.License.InfoUrl = result.Random.License.InfoUrl;

            return new SignedGenerationInfo<SignedBlobsRandom, byte[]>(
                random, result.BitsUsed, result.BitsLeft, result.RequestsLeft, result.Signature);
        }
    }
}