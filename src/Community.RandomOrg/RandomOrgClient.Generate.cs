using System;
using System.Data.JsonRpc;
using System.Threading;
using System.Threading.Tasks;
using Community.RandomOrg.Data;

namespace Community.RandomOrg
{
    partial class RandomOrgClient
    {
        /// <summary>Generates true random integers within a user-defined range as an asynchronous operation.</summary>
        /// <param name="count">How many random integers to generate. Must be within the [1,1e4] range.</param>
        /// <param name="minimum">The lower boundary for the range from which the random numbers will be picked. Must be within the [-1e9,1e9] range.</param>
        /// <param name="maximum">The upper boundary for the range from which the random numbers will be picked. Must be within the [-1e9,1e9] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="minimum" />, or <paramref name="maximum" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        public Task<SimpleGenerationInfo<int>> GenerateIntegersAsync(
            int count, int minimum, int maximum, bool replacement)
        {
            return GenerateIntegersAsync(count, minimum, maximum, replacement, CancellationToken.None);
        }

        /// <summary>Generates true random integers within a user-defined range as an asynchronous operation.</summary>
        /// <param name="count">How many random integers to generate. Must be within the [1,1e4] range.</param>
        /// <param name="minimum">The lower boundary for the range from which the random numbers will be picked. Must be within the [-1e9,1e9] range.</param>
        /// <param name="maximum">The upper boundary for the range from which the random numbers will be picked. Must be within the [-1e9,1e9] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="minimum" />, or <paramref name="maximum" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<SimpleGenerationInfo<int>> GenerateIntegersAsync(
            int count, int minimum, int maximum, bool replacement, CancellationToken cancellationToken)
        {
            if ((count < 1) || (count > 10000))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, _resourceManager.GetString("RandomIntegersCountError"));
            }
            if ((minimum < -1000000000) || (minimum > 1000000000))
            {
                throw new ArgumentOutOfRangeException(nameof(minimum), minimum, _resourceManager.GetString("RandomIntegersMinimumError"));
            }
            if ((maximum < -1000000000) || (maximum > 1000000000))
            {
                throw new ArgumentOutOfRangeException(nameof(maximum), maximum, _resourceManager.GetString("RandomIntegersMaximumError"));
            }

            EnsureApiKeyIsSpecified();

            var @params = new RpcGenerateIntegersParams
            {
                ApiKey = _apiKey,
                Count = count,
                Minimum = minimum,
                Maximum = maximum,
                Replacement = replacement
            };

            var result = await InvokeRandomOrgMethod<RpcSimpleRandomResult<int>, RpcSimpleRandom<int>, int>(
                _RPC_GENERATE_SIMPLE_INTEGERS, @params, cancellationToken).ConfigureAwait(false);

            var random = new SimpleRandom<int>();

            TransferValues(result.Random, random);

            return new SimpleGenerationInfo<int>(
                random, result.BitsUsed, result.BitsLeft, result.RequestsLeft);
        }

        /// <summary>Generates true random decimal fractions from a uniform distribution across the [0,1] interval with a user-defined number of decimal places as an asynchronous operation.</summary>
        /// <param name="count">How many random decimal fractions to generate. Must be within the [1,1e4] range.</param>
        /// <param name="decimalPlaces">The number of decimal places to use. Must be within the [1,20] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="decimalPlaces" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        public Task<SimpleGenerationInfo<decimal>> GenerateDecimalFractionsAsync(
            int count, int decimalPlaces, bool replacement)
        {
            return GenerateDecimalFractionsAsync(count, decimalPlaces, replacement, CancellationToken.None);
        }

        /// <summary>Generates true random decimal fractions from a uniform distribution across the [0,1] interval with a user-defined number of decimal places as an asynchronous operation.</summary>
        /// <param name="count">How many random decimal fractions to generate. Must be within the [1,1e4] range.</param>
        /// <param name="decimalPlaces">The number of decimal places to use. Must be within the [1,20] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="decimalPlaces" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<SimpleGenerationInfo<decimal>> GenerateDecimalFractionsAsync(
            int count, int decimalPlaces, bool replacement, CancellationToken cancellationToken)
        {
            if ((count < 1) || (count > 10000))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, _resourceManager.GetString("RandomDecimalFractionsCountError"));
            }
            if ((decimalPlaces < 1) || (decimalPlaces > 20))
            {
                throw new ArgumentOutOfRangeException(nameof(decimalPlaces), decimalPlaces, _resourceManager.GetString("RandomDecimalFractionsDecimalPlacesError"));
            }

            EnsureApiKeyIsSpecified();

            var @params = new RpcGenerateDecimalFractionsParams
            {
                ApiKey = _apiKey,
                Count = count,
                DecimalPlaces = decimalPlaces,
                Replacement = replacement
            };

            var result = await InvokeRandomOrgMethod<RpcSimpleRandomResult<decimal>, RpcSimpleRandom<decimal>, decimal>(
                _RPC_GENERATE_SIMPLE_DECIMAL_FRACTIONS, @params, cancellationToken).ConfigureAwait(false);

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
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="mean" />, <paramref name="standardDeviation" />, or <paramref name="significantDigits" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        public Task<SimpleGenerationInfo<decimal>> GenerateGaussiansAsync(
            int count, decimal mean, decimal standardDeviation, int significantDigits)
        {
            return GenerateGaussiansAsync(count, mean, standardDeviation, significantDigits, CancellationToken.None);
        }

        /// <summary>Generates true random numbers from a Gaussian distribution as an asynchronous operation.</summary>
        /// <param name="count">How many random numbers to generate. Must be within the [1,1e4] range.</param>
        /// <param name="mean">The distribution's mean. Must be within the [-1e6,1e6] range.</param>
        /// <param name="standardDeviation">The distribution's standard deviation. Must be within the [-1e6,1e6] range.</param>
        /// <param name="significantDigits">The number of significant digits to use. Must be within the [2,20] range.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="mean" />, <paramref name="standardDeviation" />, or <paramref name="significantDigits" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<SimpleGenerationInfo<decimal>> GenerateGaussiansAsync(
            int count, decimal mean, decimal standardDeviation, int significantDigits, CancellationToken cancellationToken)
        {
            if ((count < 1) || (count > 10000))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, _resourceManager.GetString("RandomGaussiansCountError"));
            }
            if ((mean < -1000000) || (mean > 1000000))
            {
                throw new ArgumentOutOfRangeException(nameof(mean), mean, _resourceManager.GetString("RandomGaussiansMeanError"));
            }
            if ((standardDeviation < -1000000) || (standardDeviation > 1000000))
            {
                throw new ArgumentOutOfRangeException(nameof(standardDeviation), standardDeviation, _resourceManager.GetString("RandomGaussiansStandardDeviationError"));
            }
            if ((significantDigits < 2) || (significantDigits > 20))
            {
                throw new ArgumentOutOfRangeException(nameof(significantDigits), significantDigits, _resourceManager.GetString("RandomGaussiansSignificantDigitsError"));
            }

            EnsureApiKeyIsSpecified();

            var @params = new RpcGenerateGaussiansParams
            {
                ApiKey = _apiKey,
                Count = count,
                Mean = mean,
                StandardDeviation = standardDeviation,
                SignificantDigits = significantDigits
            };

            var result = await InvokeRandomOrgMethod<RpcSimpleRandomResult<decimal>, RpcSimpleRandom<decimal>, decimal>(
                _RPC_GENERATE_SIMPLE_GAUSSIANS, @params, cancellationToken).ConfigureAwait(false);

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
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="length" /> is outside the allowable range of values.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="characters" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="characters" /> contains invalid number of characters .</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        public Task<SimpleGenerationInfo<string>> GenerateStringsAsync(
            int count, int length, string characters, bool replacement)
        {

            return GenerateStringsAsync(count, length, characters, replacement, CancellationToken.None);
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
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<SimpleGenerationInfo<string>> GenerateStringsAsync(
            int count, int length, string characters, bool replacement, CancellationToken cancellationToken)
        {
            if ((count < 1) || (count > 10000))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, _resourceManager.GetString("RandomStringsCountError"));
            }
            if ((length < 1) || (length > 20))
            {
                throw new ArgumentOutOfRangeException(nameof(length), length, _resourceManager.GetString("RandomStringsLengthError"));
            }
            if (characters == null)
            {
                throw new ArgumentNullException(nameof(characters));
            }
            if ((characters.Length < 1) || (characters.Length > 80))
            {
                throw new ArgumentException(_resourceManager.GetString("RandomStringsCharactersNumberError"), nameof(characters));
            }

            EnsureApiKeyIsSpecified();

            var @params = new RpcGenerateStringsParams
            {
                ApiKey = _apiKey,
                Count = count,
                Length = length,
                Characters = characters,
                Replacement = replacement
            };

            var result = await InvokeRandomOrgMethod<RpcSimpleRandomResult<string>, RpcSimpleRandom<string>, string>(
                _RPC_GENERATE_SIMPLE_STRINGS, @params, cancellationToken).ConfigureAwait(false);

            var random = new SimpleRandom<string>();

            TransferValues(result.Random, random);

            return new SimpleGenerationInfo<string>(
                random, result.BitsUsed, result.BitsLeft, result.RequestsLeft);
        }

        /// <summary>Generates version 4 true random UUIDs in accordance with section 4.4 of RFC 4122 as an asynchronous operation.</summary>
        /// <param name="count">How many random UUIDs to generate. Must be within the [1,1e3] range.</param>
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        public Task<SimpleGenerationInfo<Guid>> GenerateUuidsAsync(
            int count)
        {
            return GenerateUuidsAsync(count, CancellationToken.None);
        }

        /// <summary>Generates version 4 true random UUIDs in accordance with section 4.4 of RFC 4122 as an asynchronous operation.</summary>
        /// <param name="count">How many random UUIDs to generate. Must be within the [1,1e3] range.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<SimpleGenerationInfo<Guid>> GenerateUuidsAsync(
            int count, CancellationToken cancellationToken)
        {
            if ((count < 1) || (count > 1000))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, _resourceManager.GetString("RandomUuidsCountError"));
            }

            EnsureApiKeyIsSpecified();

            var @params = new RpcGenerateUuidsParams
            {
                ApiKey = _apiKey,
                Count = count
            };

            var result = await InvokeRandomOrgMethod<RpcSimpleRandomResult<Guid>, RpcSimpleRandom<Guid>, Guid>(
                _RPC_GENERATE_SIMPLE_UUIDS, @params, cancellationToken).ConfigureAwait(false);

            var random = new SimpleRandom<Guid>();

            TransferValues(result.Random, random);

            return new SimpleGenerationInfo<Guid>(
                random, result.BitsUsed, result.BitsLeft, result.RequestsLeft);
        }

        /// <summary>Generates BLOBs containing true random data as an asynchronous operation.</summary>
        /// <param name="count">How many random blobs to generate. Must be within the [1,100] range.</param>
        /// <param name="size">The size of each blob, measured in bits. Must be within the [1,1048576] range and must be divisible by 8. The total size of all blobs requested must not exceed 1048576 bits.</param>
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="size" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        public Task<SimpleGenerationInfo<byte[]>> GenerateBlobsAsync(
            int count, int size)
        {
            return GenerateBlobsAsync(count, size, CancellationToken.None);
        }

        /// <summary>Generates BLOBs containing true random data as an asynchronous operation.</summary>
        /// <param name="count">How many random blobs to generate. Must be within the [1,100] range.</param>
        /// <param name="size">The size of each blob, measured in bits. Must be within the [1,1048576] range and must be divisible by 8. The total size of all blobs requested must not exceed 1048576 bits.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="size" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<SimpleGenerationInfo<byte[]>> GenerateBlobsAsync(
            int count, int size, CancellationToken cancellationToken)
        {
            if ((count < 1) || (count > 100))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, _resourceManager.GetString("RandomBlobsCountError"));
            }
            if ((size < 1) || (size > 1048576))
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, _resourceManager.GetString("RandomBlobsSizeError"));
            }
            if (size % 8 != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, _resourceManager.GetString("RandomBlobsSizeDivisionError"));
            }
            if (count * size > 1048576)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, _resourceManager.GetString("RandomBlobsTotalSizeError"));
            }

            EnsureApiKeyIsSpecified();

            var @params = new RpcGenerateBlobsParams
            {
                ApiKey = _apiKey,
                Count = count,
                Size = size
            };

            var result = await InvokeRandomOrgMethod<RpcSimpleRandomResult<string>, RpcSimpleRandom<string>, string>(
                _RPC_GENERATE_SIMPLE_BLOBS, @params, cancellationToken).ConfigureAwait(false);

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
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="minimum" />, or <paramref name="maximum" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        public Task<SignedGenerationInfo<SignedIntegersRandom, int>> GenerateSignedIntegersAsync(
            int count, int minimum, int maximum, bool replacement)
        {
            return GenerateSignedIntegersAsync(count, minimum, maximum, replacement, null, CancellationToken.None);
        }

        /// <summary>Generates true random integers within a user-defined range with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random integers to generate. Must be within the [1,1e4] range.</param>
        /// <param name="minimum">The lower boundary for the range from which the random numbers will be picked. Must be within the [-1e9,1e9] range.</param>
        /// <param name="maximum">The upper boundary for the range from which the random numbers will be picked. Must be within the [-1e9,1e9] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="minimum" />, or <paramref name="maximum" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public Task<SignedGenerationInfo<SignedIntegersRandom, int>> GenerateSignedIntegersAsync(
            int count, int minimum, int maximum, bool replacement, CancellationToken cancellationToken)
        {
            return GenerateSignedIntegersAsync(count, minimum, maximum, replacement, null, cancellationToken);
        }

        /// <summary>Generates true random integers within a user-defined range with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random integers to generate. Must be within the [1,1e4] range.</param>
        /// <param name="minimum">The lower boundary for the range from which the random numbers will be picked. Must be within the [-1e9,1e9] range.</param>
        /// <param name="maximum">The upper boundary for the range from which the random numbers will be picked. Must be within the [-1e9,1e9] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1e3.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="minimum" />, or <paramref name="maximum" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public Task<SignedGenerationInfo<SignedIntegersRandom, int>> GenerateSignedIntegersAsync(
            int count, int minimum, int maximum, bool replacement, string userData)
        {
            return GenerateSignedIntegersAsync(count, minimum, maximum, replacement, userData, CancellationToken.None);
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
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<SignedGenerationInfo<SignedIntegersRandom, int>> GenerateSignedIntegersAsync(
            int count, int minimum, int maximum, bool replacement, string userData, CancellationToken cancellationToken)
        {
            if ((count < 1) || (count > 10000))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, _resourceManager.GetString("RandomIntegersCountError"));
            }
            if ((minimum < -1000000000) || (minimum > 1000000000))
            {
                throw new ArgumentOutOfRangeException(nameof(minimum), minimum, _resourceManager.GetString("RandomIntegersMinimumError"));
            }
            if ((maximum < -1000000000) || (maximum > 1000000000))
            {
                throw new ArgumentOutOfRangeException(nameof(maximum), maximum, _resourceManager.GetString("RandomIntegersMaximumError"));
            }
            if ((userData != null) && (userData.Length > 1000))
            {
                throw new ArgumentException(_resourceManager.GetString("RandomUserDataLengthError"), nameof(userData));
            }

            EnsureApiKeyIsSpecified();

            var @params = new RpcGenerateIntegersParams
            {
                ApiKey = _apiKey,
                UserData = userData,
                Count = count,
                Minimum = minimum,
                Maximum = maximum,
                Replacement = replacement
            };

            var result = await InvokeRandomOrgMethod<RpcSignedRandomResult<RpcSignedIntegersRandom, int>, RpcSignedIntegersRandom, int>(
                _RPC_GENERATE_SIGNED_INTEGERS, @params, cancellationToken).ConfigureAwait(false);

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
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="decimalPlaces" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        public Task<SignedGenerationInfo<SignedDecimalFractionsRandom, decimal>> GenerateSignedDecimalFractionsAsync(
            int count, int decimalPlaces, bool replacement)
        {
            return GenerateSignedDecimalFractionsAsync(count, decimalPlaces, replacement, null, CancellationToken.None);
        }

        /// <summary>Generates true random decimal fractions from a uniform distribution across the [0,1] interval with a user-defined number of decimal places with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random decimal fractions to generate. Must be within the [1,1e4] range.</param>
        /// <param name="decimalPlaces">The number of decimal places to use. Must be within the [1,20] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="decimalPlaces" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public Task<SignedGenerationInfo<SignedDecimalFractionsRandom, decimal>> GenerateSignedDecimalFractionsAsync(
            int count, int decimalPlaces, bool replacement, CancellationToken cancellationToken)
        {
            return GenerateSignedDecimalFractionsAsync(count, decimalPlaces, replacement, null, cancellationToken);
        }

        /// <summary>Generates true random decimal fractions from a uniform distribution across the [0,1] interval with a user-defined number of decimal places with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random decimal fractions to generate. Must be within the [1,1e4] range.</param>
        /// <param name="decimalPlaces">The number of decimal places to use. Must be within the [1,20] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1e3.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="decimalPlaces" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public Task<SignedGenerationInfo<SignedDecimalFractionsRandom, decimal>> GenerateSignedDecimalFractionsAsync(
            int count, int decimalPlaces, bool replacement, string userData)
        {
            return GenerateSignedDecimalFractionsAsync(count, decimalPlaces, replacement, userData, CancellationToken.None);
        }

        /// <summary>Generates true random decimal fractions from a uniform distribution across the [0,1] interval with a user-defined number of decimal places with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random decimal fractions to generate. Must be within the [1,1e4] range.</param>
        /// <param name="decimalPlaces">The number of decimal places to use. Must be within the [1,20] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1e3.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="decimalPlaces" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<SignedGenerationInfo<SignedDecimalFractionsRandom, decimal>> GenerateSignedDecimalFractionsAsync(
            int count, int decimalPlaces, bool replacement, string userData, CancellationToken cancellationToken)
        {
            if ((count < 1) || (count > 10000))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, _resourceManager.GetString("RandomDecimalFractionsCountError"));
            }
            if ((decimalPlaces < 1) || (decimalPlaces > 20))
            {
                throw new ArgumentOutOfRangeException(nameof(decimalPlaces), decimalPlaces, _resourceManager.GetString("RandomDecimalFractionsDecimalPlacesError"));
            }
            if ((userData != null) && (userData.Length > 1000))
            {
                throw new ArgumentException(_resourceManager.GetString("RandomUserDataLengthError"), nameof(userData));
            }

            EnsureApiKeyIsSpecified();

            var @params = new RpcGenerateDecimalFractionsParams
            {
                ApiKey = _apiKey,
                UserData = userData,
                Count = count,
                DecimalPlaces = decimalPlaces,
                Replacement = replacement
            };

            var result = await InvokeRandomOrgMethod<RpcSignedRandomResult<RpcSignedDecimalFractionsRandom, decimal>, RpcSignedDecimalFractionsRandom, decimal>(
                _RPC_GENERATE_SIGNED_DECIMAL_FRACTIONS, @params, cancellationToken).ConfigureAwait(false);

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
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="mean" />, <paramref name="standardDeviation" />, or <paramref name="significantDigits" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        public Task<SignedGenerationInfo<SignedGaussiansRandom, decimal>> GenerateSignedGaussiansAsync(
            int count, decimal mean, decimal standardDeviation, int significantDigits)
        {
            return GenerateSignedGaussiansAsync(count, mean, standardDeviation, significantDigits, null, CancellationToken.None);
        }

        /// <summary>Generates true random numbers from a Gaussian distribution with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random numbers to generate. Must be within the [1,1e4] range.</param>
        /// <param name="mean">The distribution's mean. Must be within the [-1e6,1e6] range.</param>
        /// <param name="standardDeviation">The distribution's standard deviation. Must be within the [-1e6,1e6] range.</param>
        /// <param name="significantDigits">The number of significant digits to use. Must be within the [2,20] range.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="mean" />, <paramref name="standardDeviation" />, or <paramref name="significantDigits" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public Task<SignedGenerationInfo<SignedGaussiansRandom, decimal>> GenerateSignedGaussiansAsync(
            int count, decimal mean, decimal standardDeviation, int significantDigits, CancellationToken cancellationToken)
        {
            return GenerateSignedGaussiansAsync(count, mean, standardDeviation, significantDigits, null, cancellationToken);
        }

        /// <summary>Generates true random numbers from a Gaussian distribution with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random numbers to generate. Must be within the [1,1e4] range.</param>
        /// <param name="mean">The distribution's mean. Must be within the [-1e6,1e6] range.</param>
        /// <param name="standardDeviation">The distribution's standard deviation. Must be within the [-1e6,1e6] range.</param>
        /// <param name="significantDigits">The number of significant digits to use. Must be within the [2,20] range.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1e3.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="mean" />, <paramref name="standardDeviation" />, or <paramref name="significantDigits" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public Task<SignedGenerationInfo<SignedGaussiansRandom, decimal>> GenerateSignedGaussiansAsync(
            int count, decimal mean, decimal standardDeviation, int significantDigits, string userData)
        {
            return GenerateSignedGaussiansAsync(count, mean, standardDeviation, significantDigits, userData, CancellationToken.None);
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
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<SignedGenerationInfo<SignedGaussiansRandom, decimal>> GenerateSignedGaussiansAsync(
            int count, decimal mean, decimal standardDeviation, int significantDigits, string userData, CancellationToken cancellationToken)
        {
            if ((count < 1) || (count > 10000))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, _resourceManager.GetString("RandomGaussiansCountError"));
            }
            if ((mean < -1000000) || (mean > 1000000))
            {
                throw new ArgumentOutOfRangeException(nameof(mean), mean, _resourceManager.GetString("RandomGaussiansMeanError"));
            }
            if ((standardDeviation < -1000000) || (standardDeviation > 1000000))
            {
                throw new ArgumentOutOfRangeException(nameof(standardDeviation), standardDeviation, _resourceManager.GetString("RandomGaussiansStandardDeviationError"));
            }
            if ((significantDigits < 2) || (significantDigits > 20))
            {
                throw new ArgumentOutOfRangeException(nameof(significantDigits), significantDigits, _resourceManager.GetString("RandomGaussiansSignificantDigitsError"));
            }
            if ((userData != null) && (userData.Length > 1000))
            {
                throw new ArgumentException(_resourceManager.GetString("RandomUserDataLengthError"), nameof(userData));
            }

            EnsureApiKeyIsSpecified();

            var @params = new RpcGenerateGaussiansParams
            {
                ApiKey = _apiKey,
                UserData = userData,
                Count = count,
                Mean = mean,
                StandardDeviation = standardDeviation,
                SignificantDigits = significantDigits
            };

            var result = await InvokeRandomOrgMethod<RpcSignedRandomResult<RpcSignedGaussiansRandom, decimal>, RpcSignedGaussiansRandom, decimal>(
                _RPC_GENERATE_SIGNED_GAUSSIANS, @params, cancellationToken).ConfigureAwait(false);

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
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="length" /> is outside the allowable range of values.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="characters" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="characters" /> contains invalid number of characters .</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        public Task<SignedGenerationInfo<SignedStringsRandom, string>> GenerateSignedStringsAsync(
            int count, int length, string characters, bool replacement)
        {
            return GenerateSignedStringsAsync(count, length, characters, replacement, null, CancellationToken.None);
        }

        /// <summary>Generates true random strings with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random strings to generate. Must be within the [1,1e4] range.</param>
        /// <param name="length">The length of each string. Must be within the [1,20] range.</param>
        /// <param name="characters">A string that contains the set of characters that are allowed to occur in the random strings. The maximum number of characters is 80.</param>
        /// <param name="replacement">Specifies whether the random strings should be picked with replacement.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="length" /> is outside the allowable range of values.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="characters" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="characters" /> contains invalid number of characters .</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public Task<SignedGenerationInfo<SignedStringsRandom, string>> GenerateSignedStringsAsync(
            int count, int length, string characters, bool replacement, CancellationToken cancellationToken)
        {
            return GenerateSignedStringsAsync(count, length, characters, replacement, null, cancellationToken);
        }

        /// <summary>Generates true random strings with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random strings to generate. Must be within the [1,1e4] range.</param>
        /// <param name="length">The length of each string. Must be within the [1,20] range.</param>
        /// <param name="characters">A string that contains the set of characters that are allowed to occur in the random strings. The maximum number of characters is 80.</param>
        /// <param name="replacement">Specifies whether the random strings should be picked with replacement.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1e3.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="length" /> is outside the allowable range of values.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="characters" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="characters" /> contains invalid number of characters .</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public Task<SignedGenerationInfo<SignedStringsRandom, string>> GenerateSignedStringsAsync(
            int count, int length, string characters, bool replacement, string userData)
        {
            return GenerateSignedStringsAsync(count, length, characters, replacement, userData, CancellationToken.None);
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
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<SignedGenerationInfo<SignedStringsRandom, string>> GenerateSignedStringsAsync(
            int count, int length, string characters, bool replacement, string userData, CancellationToken cancellationToken)
        {
            if ((count < 1) || (count > 10000))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, _resourceManager.GetString("RandomStringsCountError"));
            }
            if ((length < 1) || (length > 20))
            {
                throw new ArgumentOutOfRangeException(nameof(length), length, _resourceManager.GetString("RandomStringsLengthError"));
            }
            if (characters == null)
            {
                throw new ArgumentNullException(nameof(characters));
            }
            if ((characters.Length < 1) || (characters.Length > 80))
            {
                throw new ArgumentException(_resourceManager.GetString("RandomStringsCharactersNumberError"), nameof(characters));
            }
            if ((userData != null) && (userData.Length > 1000))
            {
                throw new ArgumentException(_resourceManager.GetString("RandomUserDataLengthError"), nameof(userData));
            }

            EnsureApiKeyIsSpecified();

            var @params = new RpcGenerateStringsParams
            {
                ApiKey = _apiKey,
                UserData = userData,
                Count = count,
                Length = length,
                Characters = characters,
                Replacement = replacement
            };

            var result = await InvokeRandomOrgMethod<RpcSignedRandomResult<RpcSignedStringsRandom, string>, RpcSignedStringsRandom, string>(
                _RPC_GENERATE_SIGNED_STRINGS, @params, cancellationToken).ConfigureAwait(false);

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
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        public Task<SignedGenerationInfo<SignedUuidsRandom, Guid>> GenerateSignedUuidsAsync(
            int count)
        {
            return GenerateSignedUuidsAsync(count, null, CancellationToken.None);
        }

        /// <summary>Generates version 4 true random UUIDs in accordance with section 4.4 of RFC 4122 with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random UUIDs to generate. Must be within the [1,1e3] range.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public Task<SignedGenerationInfo<SignedUuidsRandom, Guid>> GenerateSignedUuidsAsync(
            int count, CancellationToken cancellationToken)
        {
            return GenerateSignedUuidsAsync(count, null, cancellationToken);
        }

        /// <summary>Generates version 4 true random UUIDs in accordance with section 4.4 of RFC 4122 with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random UUIDs to generate. Must be within the [1,1e3] range.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1e3.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public Task<SignedGenerationInfo<SignedUuidsRandom, Guid>> GenerateSignedUuidsAsync(
            int count, string userData)
        {
            return GenerateSignedUuidsAsync(count, userData, CancellationToken.None);
        }

        /// <summary>Generates version 4 true random UUIDs in accordance with section 4.4 of RFC 4122 with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random UUIDs to generate. Must be within the [1,1e3] range.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1e3.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<SignedGenerationInfo<SignedUuidsRandom, Guid>> GenerateSignedUuidsAsync(
            int count, string userData, CancellationToken cancellationToken)
        {
            if ((count < 1) || (count > 1000))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, _resourceManager.GetString("RandomUuidsCountError"));
            }
            if ((userData != null) && (userData.Length > 1000))
            {
                throw new ArgumentException(_resourceManager.GetString("RandomUserDataLengthError"), nameof(userData));
            }

            EnsureApiKeyIsSpecified();

            var @params = new RpcGenerateUuidsParams
            {
                ApiKey = _apiKey,
                UserData = userData,
                Count = count
            };

            var result = await InvokeRandomOrgMethod<RpcSignedRandomResult<RpcSignedUuidsRandom, Guid>, RpcSignedUuidsRandom, Guid>(
                _RPC_GENERATE_SIGNED_UUIDS, @params, cancellationToken).ConfigureAwait(false);

            var random = new SignedUuidsRandom();

            TransferValues(result.Random, random);

            return new SignedGenerationInfo<SignedUuidsRandom, Guid>(
                random, result.BitsUsed, result.BitsLeft, result.RequestsLeft, result.Signature);
        }

        /// <summary>Generates BLOBs containing true random data with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random blobs to generate. Must be within the [1,100] range.</param>
        /// <param name="size">The size of each blob, measured in bits. Must be within the [1,1048576] range and must be divisible by 8. The total size of all blobs requested must not exceed 1048576 bits.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="size" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        public Task<SignedGenerationInfo<SignedBlobsRandom, byte[]>> GenerateSignedBlobsAsync(
            int count, int size)
        {
            return GenerateSignedBlobsAsync(count, size, null, CancellationToken.None);
        }

        /// <summary>Generates BLOBs containing true random data with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random blobs to generate. Must be within the [1,100] range.</param>
        /// <param name="size">The size of each blob, measured in bits. Must be within the [1,1048576] range and must be divisible by 8. The total size of all blobs requested must not exceed 1048576 bits.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="size" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public Task<SignedGenerationInfo<SignedBlobsRandom, byte[]>> GenerateSignedBlobsAsync(
            int count, int size, CancellationToken cancellationToken)
        {
            return GenerateSignedBlobsAsync(count, size, null, cancellationToken);
        }

        /// <summary>Generates BLOBs containing true random data with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random blobs to generate. Must be within the [1,100] range.</param>
        /// <param name="size">The size of each blob, measured in bits. Must be within the [1,1048576] range and must be divisible by 8. The total size of all blobs requested must not exceed 1048576 bits.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1e3.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="size" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public Task<SignedGenerationInfo<SignedBlobsRandom, byte[]>> GenerateSignedBlobsAsync(
            int count, int size, string userData)
        {
            return GenerateSignedBlobsAsync(count, size, userData, CancellationToken.None);
        }

        /// <summary>Generates BLOBs containing true random data with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random blobs to generate. Must be within the [1,100] range.</param>
        /// <param name="size">The size of each blob, measured in bits. Must be within the [1,1048576] range and must be divisible by 8. The total size of all blobs requested must not exceed 1048576 bits.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1e3.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="size" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<SignedGenerationInfo<SignedBlobsRandom, byte[]>> GenerateSignedBlobsAsync(
            int count, int size, string userData, CancellationToken cancellationToken)
        {
            if ((count < 1) || (count > 100))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, _resourceManager.GetString("RandomBlobsCountError"));
            }
            if ((size < 1) || (size > 1048576))
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, _resourceManager.GetString("RandomBlobsSizeError"));
            }
            if (size % 8 != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, _resourceManager.GetString("RandomBlobsSizeDivisionError"));
            }
            if (count * size > 1048576)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, _resourceManager.GetString("RandomBlobsTotalSizeError"));
            }
            if ((userData != null) && (userData.Length > 1000))
            {
                throw new ArgumentException(_resourceManager.GetString("RandomUserDataLengthError"), nameof(userData));
            }

            EnsureApiKeyIsSpecified();

            var @params = new RpcGenerateBlobsParams
            {
                ApiKey = _apiKey,
                UserData = userData,
                Count = count,
                Size = size
            };

            var result = await InvokeRandomOrgMethod<RpcSignedRandomResult<RpcSignedBlobsRandom, string>, RpcSignedBlobsRandom, string>(
                _RPC_GENERATE_SIGNED_BLOBS, @params, cancellationToken).ConfigureAwait(false);

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

            random.License = new License
            {
                Type = result.Random.License.Type,
                Text = result.Random.License.Text,
                InfoUrl = result.Random.License.InfoUrl
            };

            return new SignedGenerationInfo<SignedBlobsRandom, byte[]>(
                random, result.BitsUsed, result.BitsLeft, result.RequestsLeft, result.Signature);
        }

        /// <summary>Retrieves previously generated signed results (which are stored for 24 hours) as an asynchronous operation.</summary>
        /// <typeparam name="TRandom">The type of random data container.</typeparam>
        /// <typeparam name="TValue">The type of random object.</typeparam>
        /// <param name="serialNumber">The integer containing the serial number associated with this random information.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="InvalidOperationException">An error occurred during random container conversion.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public Task<SignedGenerationInfo<TRandom, TValue>> GetResultAsync<TRandom, TValue>(long serialNumber)
            where TRandom : SignedRandom<TValue>
        {
            return GetResultAsync<TRandom, TValue>(serialNumber, CancellationToken.None);
        }

        /// <summary>Retrieves previously generated signed results (which are stored for 24 hours) as an asynchronous operation.</summary>
        /// <typeparam name="TRandom">The type of random data container.</typeparam>
        /// <typeparam name="TValue">The type of random object.</typeparam>
        /// <param name="serialNumber">The integer containing the serial number associated with this random information.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="InvalidOperationException">An error occurred during random container conversion.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<SignedGenerationInfo<TRandom, TValue>> GetResultAsync<TRandom, TValue>(long serialNumber, CancellationToken cancellationToken)
            where TRandom : SignedRandom<TValue>
        {
            EnsureApiKeyIsSpecified();

            var @params = new RpcGetResultParams
            {
                ApiKey = _apiKey,
                SerialNumber = serialNumber
            };

            var resultType = default(Type);

            if (typeof(TRandom) == typeof(SignedIntegersRandom))
            {
                resultType = typeof(RpcSignedRandomResult<RpcSignedIntegersRandom, int>);
            }
            else if (typeof(TRandom) == typeof(SignedDecimalFractionsRandom))
            {
                resultType = typeof(RpcSignedRandomResult<RpcSignedDecimalFractionsRandom, decimal>);
            }
            else if (typeof(TRandom) == typeof(SignedGaussiansRandom))
            {
                resultType = typeof(RpcSignedRandomResult<RpcSignedGaussiansRandom, decimal>);
            }
            else if (typeof(TRandom) == typeof(SignedStringsRandom))
            {
                resultType = typeof(RpcSignedRandomResult<RpcSignedStringsRandom, string>);
            }
            else if (typeof(TRandom) == typeof(SignedUuidsRandom))
            {
                resultType = typeof(RpcSignedRandomResult<RpcSignedUuidsRandom, Guid>);
            }
            else if (typeof(TRandom) == typeof(SignedBlobsRandom))
            {
                resultType = typeof(RpcSignedRandomResult<RpcSignedBlobsRandom, string>);
            }

            var result = default(object);

            try
            {
                result = await InvokeRandomOrgMethod(_RPC_GET_RESULT, @params, resultType, cancellationToken);
            }
            catch (JsonRpcException ex)
                when (ex.Type == JsonRpcExceptionType.GenericError)
            {
                throw new InvalidOperationException(_resourceManager.GetString("ServiceRandomContainerTypeError"), ex);
            }

            var generationInfo = default(SignedGenerationInfo<TRandom, TValue>);

            if (typeof(TRandom) == typeof(SignedIntegersRandom))
            {
                var typedResult = (RpcSignedRandomResult<RpcSignedIntegersRandom, int>)result;

                var typedRandom = new SignedIntegersRandom
                {
                    Minimum = (int)typedResult.Random.Minimum,
                    Maximum = (int)typedResult.Random.Maximum,
                    Replacement = typedResult.Random.Replacement
                };

                TransferValues(typedResult.Random, typedRandom);

                var typedGenerationInfo = new SignedGenerationInfo<SignedIntegersRandom, int>(
                    typedRandom, typedResult.BitsUsed, typedResult.BitsLeft, typedResult.RequestsLeft, typedResult.Signature);

                generationInfo = (SignedGenerationInfo<TRandom, TValue>)(object)typedGenerationInfo;
            }
            else if (typeof(TRandom) == typeof(SignedDecimalFractionsRandom))
            {
                var typedResult = (RpcSignedRandomResult<RpcSignedDecimalFractionsRandom, decimal>)result;

                var typedRandom = new SignedDecimalFractionsRandom
                {
                    DecimalPlaces = (int)typedResult.Random.DecimalPlaces,
                    Replacement = typedResult.Random.Replacement
                };

                TransferValues(typedResult.Random, typedRandom);

                var typedGenerationInfo = new SignedGenerationInfo<SignedDecimalFractionsRandom, decimal>(
                    typedRandom, typedResult.BitsUsed, typedResult.BitsLeft, typedResult.RequestsLeft, typedResult.Signature);

                generationInfo = (SignedGenerationInfo<TRandom, TValue>)(object)typedGenerationInfo;
            }
            else if (typeof(TRandom) == typeof(SignedGaussiansRandom))
            {
                var typedResult = (RpcSignedRandomResult<RpcSignedGaussiansRandom, decimal>)result;

                var typedRandom = new SignedGaussiansRandom
                {
                    Mean = typedResult.Random.Mean,
                    StandardDeviation = typedResult.Random.StandardDeviation,
                    SignificantDigits = (int)typedResult.Random.SignificantDigits
                };

                TransferValues(typedResult.Random, typedRandom);

                var typedGenerationInfo = new SignedGenerationInfo<SignedGaussiansRandom, decimal>(
                    typedRandom, typedResult.BitsUsed, typedResult.BitsLeft, typedResult.RequestsLeft, typedResult.Signature);

                generationInfo = (SignedGenerationInfo<TRandom, TValue>)(object)typedGenerationInfo;
            }
            else if (typeof(TRandom) == typeof(SignedStringsRandom))
            {
                var typedResult = (RpcSignedRandomResult<RpcSignedStringsRandom, string>)result;

                var typedRandom = new SignedStringsRandom
                {
                    Length = (int)typedResult.Random.Length,
                    Characters = typedResult.Random.Characters,
                    Replacement = typedResult.Random.Replacement
                };

                TransferValues(typedResult.Random, typedRandom);

                var typedGenerationInfo = new SignedGenerationInfo<SignedStringsRandom, string>(
                    typedRandom, typedResult.BitsUsed, typedResult.BitsLeft, typedResult.RequestsLeft, typedResult.Signature);

                generationInfo = (SignedGenerationInfo<TRandom, TValue>)(object)typedGenerationInfo;
            }
            else if (typeof(TRandom) == typeof(SignedUuidsRandom))
            {
                var typedResult = (RpcSignedRandomResult<RpcSignedUuidsRandom, Guid>)result;
                var typedRandom = new SignedUuidsRandom();

                TransferValues(typedResult.Random, typedRandom);

                var typedGenerationInfo = new SignedGenerationInfo<SignedUuidsRandom, Guid>(
                    typedRandom, typedResult.BitsUsed, typedResult.BitsLeft, typedResult.RequestsLeft, typedResult.Signature);

                generationInfo = (SignedGenerationInfo<TRandom, TValue>)(object)typedGenerationInfo;
            }
            else if (typeof(TRandom) == typeof(SignedBlobsRandom))
            {
                var typedResult = (RpcSignedRandomResult<RpcSignedBlobsRandom, string>)result;

                var typedRandom = new SignedBlobsRandom
                {
                    ApiKeyHash = typedResult.Random.ApiKeyHash,
                    CompletionTime = typedResult.Random.CompletionTime,
                    SerialNumber = typedResult.Random.SerialNumber,
                    Size = (int)typedResult.Random.Size
                };

                var data = new byte[typedResult.Random.Data.Count][];

                for (var i = 0; i < data.Length; i++)
                {
                    data[i] = Convert.FromBase64String(typedResult.Random.Data[i]);
                }

                typedRandom.Data = data;

                var typedGenerationInfo = new SignedGenerationInfo<SignedBlobsRandom, byte[]>(
                    typedRandom, typedResult.BitsUsed, typedResult.BitsLeft, typedResult.RequestsLeft, typedResult.Signature);

                generationInfo = (SignedGenerationInfo<TRandom, TValue>)(object)typedGenerationInfo;
            }

            return generationInfo;
        }

        /// <summary>Verifies the signature of signed random objects and associated data.</summary>
        /// <typeparam name="T">The type of random object.</typeparam>
        /// <param name="random">The signed random objects and associated data.</param>
        /// <param name="signature">The signature from the same response that the random data originates from.</param>
        /// <returns>A value, indicating if the random objects are authentic.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="random" /> or <paramref name="signature" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="random" /> or <paramref name="signature" /> has invalid values.</exception>
        public Task<bool> VerifySignatureAsync<T>(SignedRandom<T> random, byte[] signature)
        {
            return VerifySignatureAsync(random, signature, CancellationToken.None);
        }

        /// <summary>Verifies the signature of signed random objects and associated data.</summary>
        /// <typeparam name="T">The type of random object.</typeparam>
        /// <param name="random">The signed random objects and associated data.</param>
        /// <param name="signature">The signature from the same response that the random data originates from.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A value, indicating if the random objects are authentic.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="random" /> or <paramref name="signature" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="random" /> or <paramref name="signature" /> has invalid values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<bool> VerifySignatureAsync<T>(SignedRandom<T> random, byte[] signature, CancellationToken cancellationToken)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }
            if (signature == null)
            {
                throw new ArgumentNullException(nameof(signature));
            }
            if (random.ApiKeyHash == null)
            {
                throw new ArgumentException(_resourceManager.GetString("VerifySignatureApiKeyUndefined"), nameof(random));
            }
            if (random.ApiKeyHash.Length != 64)
            {
                throw new ArgumentException(_resourceManager.GetString("VerifySignatureApiKeyLengthError"), nameof(random));
            }
            if (random.Data == null)
            {
                throw new ArgumentException(_resourceManager.GetString("VerifySignatureDataUndefined"), nameof(random));
            }
            if (random.Data.Count == 0)
            {
                throw new ArgumentException(_resourceManager.GetString("VerifySignatureDataEmpty"), nameof(random));
            }
            if (random.CompletionTime == default(DateTime))
            {
                throw new ArgumentException(_resourceManager.GetString("VerifySignatureCompletionTimeUndefined"), nameof(random));
            }
            if (random.SerialNumber == 0L)
            {
                throw new ArgumentException(_resourceManager.GetString("VerifySignatureSerialNumberUndefined"), nameof(random));
            }
            if (random.License == null)
            {
                throw new ArgumentException(_resourceManager.GetString("VerifySignatureLicenseUndefined"), nameof(random));
            }
            if (signature.Length == 0)
            {
                throw new ArgumentException(_resourceManager.GetString("VerifySignatureSignatureEmpty"), nameof(signature));
            }

            var randomType = random.GetType();
            var @params = default(RpcVerifyParams);

            if (randomType == typeof(SignedIntegersRandom))
            {
                var typedRandom = (SignedIntegersRandom)(object)random;

                if ((typedRandom.Data.Count < 1) || (typedRandom.Data.Count > 10000))
                {
                    throw new ArgumentException(_resourceManager.GetString("RandomIntegersCountError"), nameof(random));
                }
                if ((typedRandom.Minimum < -1000000000) || (typedRandom.Minimum > 1000000000))
                {
                    throw new ArgumentException(_resourceManager.GetString("RandomIntegersMinimumError"), nameof(random));
                }
                if ((typedRandom.Maximum < -1000000000) || (typedRandom.Maximum > 1000000000))
                {
                    throw new ArgumentException(_resourceManager.GetString("RandomIntegersMaximumError"), nameof(random));
                }

                var rpcRandom = new RpcSignedIntegersRandom
                {
                    Method = _RPC_GENERATE_SIGNED_INTEGERS,
                    Minimum = typedRandom.Minimum,
                    Maximum = typedRandom.Maximum,
                    Replacement = typedRandom.Replacement,
                    Base = 10
                };

                TransferValues(typedRandom, rpcRandom);

                @params = new RpcVerifyParams<RpcSignedIntegersRandom, int>
                {
                    Random = rpcRandom
                };
            }
            else if (randomType == typeof(SignedDecimalFractionsRandom))
            {
                var typedRandom = (SignedDecimalFractionsRandom)(object)random;

                if ((typedRandom.Data.Count < 1) || (typedRandom.Data.Count > 10000))
                {
                    throw new ArgumentException(_resourceManager.GetString("RandomDecimalFractionsCountError"), nameof(random));
                }
                if ((typedRandom.DecimalPlaces < 1) || (typedRandom.DecimalPlaces > 20))
                {
                    throw new ArgumentException(_resourceManager.GetString("RandomDecimalFractionsDecimalPlacesError"), nameof(random));
                }

                var rpcRandom = new RpcSignedDecimalFractionsRandom
                {
                    Method = _RPC_GENERATE_SIGNED_DECIMAL_FRACTIONS,
                    DecimalPlaces = typedRandom.DecimalPlaces,
                    Replacement = typedRandom.Replacement
                };

                TransferValues(typedRandom, rpcRandom);

                @params = new RpcVerifyParams<RpcSignedDecimalFractionsRandom, decimal>
                {
                    Random = rpcRandom
                };
            }
            else if (randomType == typeof(SignedGaussiansRandom))
            {
                var typedRandom = (SignedGaussiansRandom)(object)random;

                if ((typedRandom.Data.Count < 1) || (typedRandom.Data.Count > 10000))
                {
                    throw new ArgumentException(_resourceManager.GetString("RandomGaussiansCountError"), nameof(random));
                }
                if ((typedRandom.Mean < -1000000) || (typedRandom.Mean > 1000000))
                {
                    throw new ArgumentException(_resourceManager.GetString("RandomGaussiansMeanError"), nameof(random));
                }
                if ((typedRandom.StandardDeviation < -1000000) || (typedRandom.StandardDeviation > 1000000))
                {
                    throw new ArgumentException(_resourceManager.GetString("RandomGaussiansStandardDeviationError"), nameof(random));
                }
                if ((typedRandom.SignificantDigits < 2) || (typedRandom.SignificantDigits > 20))
                {
                    throw new ArgumentException(_resourceManager.GetString("RandomGaussiansSignificantDigitsError"), nameof(random));
                }

                var rpcRandom = new RpcSignedGaussiansRandom
                {
                    Method = _RPC_GENERATE_SIGNED_GAUSSIANS,
                    Mean = typedRandom.Mean,
                    StandardDeviation = typedRandom.StandardDeviation,
                    SignificantDigits = typedRandom.SignificantDigits
                };

                TransferValues(typedRandom, rpcRandom);

                @params = new RpcVerifyParams<RpcSignedGaussiansRandom, decimal>
                {
                    Random = rpcRandom
                };
            }
            else if (randomType == typeof(SignedStringsRandom))
            {
                var typedRandom = (SignedStringsRandom)(object)random;

                if ((typedRandom.Data.Count < 1) || (typedRandom.Data.Count > 10000))
                {
                    throw new ArgumentException(_resourceManager.GetString("RandomStringsCountError"), nameof(random));
                }
                if ((typedRandom.Length < 1) || (typedRandom.Length > 20))
                {
                    throw new ArgumentException(_resourceManager.GetString("RandomStringsLengthError"), nameof(random));
                }
                if (typedRandom.Characters == null)
                {
                    throw new ArgumentException(_resourceManager.GetString("RandomStringsCharactersUndefined"), nameof(random));
                }
                if ((typedRandom.Characters.Length < 1) || (typedRandom.Characters.Length > 80))
                {
                    throw new ArgumentException(_resourceManager.GetString("RandomStringsCharactersNumberError"), nameof(random));
                }

                for (var i = 0; i < typedRandom.Data.Count; i++)
                {
                    if (typedRandom.Data[i] == null)
                    {
                        throw new ArgumentException(string.Format(_resourceManager.GetString("RandomStringsStringUndefined"), i), nameof(random));
                    }
                }

                var rpcRandom = new RpcSignedStringsRandom
                {
                    Method = _RPC_GENERATE_SIGNED_STRINGS,
                    Length = typedRandom.Length,
                    Characters = typedRandom.Characters,
                    Replacement = typedRandom.Replacement
                };

                TransferValues(typedRandom, rpcRandom);

                @params = new RpcVerifyParams<RpcSignedStringsRandom, string>
                {
                    Random = rpcRandom
                };
            }
            else if (randomType == typeof(SignedUuidsRandom))
            {
                var typedRandom = (SignedUuidsRandom)(object)random;

                if ((typedRandom.Data.Count < 1) || (typedRandom.Data.Count > 1000))
                {
                    throw new ArgumentException(_resourceManager.GetString("RandomUuidsCountError"), nameof(random));
                }

                var rpcRandom = new RpcSignedUuidsRandom
                {
                    Method = _RPC_GENERATE_SIGNED_UUIDS
                };

                TransferValues(typedRandom, rpcRandom);

                @params = new RpcVerifyParams<RpcSignedUuidsRandom, Guid>
                {
                    Random = rpcRandom
                };
            }
            else if (randomType == typeof(SignedBlobsRandom))
            {
                var typedRandom = (SignedBlobsRandom)(object)random;

                if ((typedRandom.Data.Count < 1) || (typedRandom.Data.Count > 100))
                {
                    throw new ArgumentException(_resourceManager.GetString("RandomBlobsCountError"), nameof(random));
                }
                if ((typedRandom.Size < 1) || (typedRandom.Size > 1048576))
                {
                    throw new ArgumentException(_resourceManager.GetString("RandomBlobsCountError"), nameof(random));
                }
                if (typedRandom.Size % 8 != 0)
                {
                    throw new ArgumentException(_resourceManager.GetString("RandomBlobsSizeDivisionError"), nameof(random));
                }
                if (typedRandom.Data.Count * typedRandom.Size > 1048576)
                {
                    throw new ArgumentException(_resourceManager.GetString("RandomBlobsTotalSizeError"), nameof(random));
                }

                var rpcRandom = new RpcSignedBlobsRandom
                {
                    Method = _RPC_GENERATE_SIGNED_BLOBS,
                    Count = typedRandom.Data.Count,
                    ApiKeyHash = typedRandom.ApiKeyHash,
                    CompletionTime = typedRandom.CompletionTime,
                    SerialNumber = typedRandom.SerialNumber,
                    Size = typedRandom.Size,
                    Format = "base64"
                };

                var rpcData = new string[typedRandom.Data.Count];

                for (var i = 0; i < typedRandom.Data.Count; i++)
                {
                    rpcData[i] = Convert.ToBase64String(typedRandom.Data[i]);
                }

                rpcRandom.Data = rpcData;

                @params = new RpcVerifyParams<RpcSignedBlobsRandom, string>
                {
                    Random = rpcRandom
                };
            }
            else
            {
                throw new NotSupportedException();
            }

            @params.Signature = signature;

            var result = await InvokeRandomOrgMethod<RpcVerifyResult>(
                _RPC_VERIFY_SIGNATUREE, @params, cancellationToken).ConfigureAwait(false);

            return result.Authenticity;
        }
    }
}