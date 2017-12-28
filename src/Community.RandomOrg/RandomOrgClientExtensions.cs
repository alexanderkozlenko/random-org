using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Community.RandomOrg.Data;

namespace Community.RandomOrg
{
    /// <summary>Aggregates extensions methods for the <see cref="RandomOrgClient" /> type.</summary>
    public static class RandomOrgClientExtensions
    {
        /// <summary>Returns information related to the usage of a given API key as an asynchronous operation.</summary>
        /// <returns>A <see cref="RandomUsage" /> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<RandomUsage> GetUsageAsync(
            this IRandomOrgClient client)
        {
            return client.GetUsageAsync(CancellationToken.None);
        }

        /// <summary>Generates true random integers within a user-defined range as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random integers to generate. Must be within the [1,1e4] range.</param>
        /// <param name="minimum">The lower boundary for the range from which the random numbers will be picked. Must be within the [-1e9,1e9] range.</param>
        /// <param name="maximum">The upper boundary for the range from which the random numbers will be picked. Must be within the [-1e9,1e9] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<SimpleGenerationInfo<int>> GenerateIntegersAsync(
            this IRandomOrgClient client, int count, int minimum, int maximum, bool replacement)
        {
            return client.GenerateIntegersAsync(count, minimum, maximum, replacement, CancellationToken.None);
        }

        /// <summary>Generates true random decimal fractions from a uniform distribution across the [0,1] interval with a user-defined number of decimal places as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random decimal fractions to generate. Must be within the [1,1e4] range.</param>
        /// <param name="decimalPlaces">The number of decimal places to use. Must be within the [1,20] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<SimpleGenerationInfo<decimal>> GenerateDecimalFractionsAsync(
            this IRandomOrgClient client, int count, int decimalPlaces, bool replacement)
        {
            return client.GenerateDecimalFractionsAsync(count, decimalPlaces, replacement, CancellationToken.None);
        }

        /// <summary>Generates true random numbers from a Gaussian distribution as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random numbers to generate. Must be within the [1,1e4] range.</param>
        /// <param name="mean">The distribution's mean. Must be within the [-1e6,1e6] range.</param>
        /// <param name="standardDeviation">The distribution's standard deviation. Must be within the [-1e6,1e6] range.</param>
        /// <param name="significantDigits">The number of significant digits to use. Must be within the [2,20] range.</param>
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<SimpleGenerationInfo<decimal>> GenerateGaussiansAsync(
            this IRandomOrgClient client, int count, decimal mean, decimal standardDeviation, int significantDigits)
        {
            return client.GenerateGaussiansAsync(count, mean, standardDeviation, significantDigits, CancellationToken.None);
        }

        /// <summary>Generates true random strings as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random strings to generate. Must be within the [1,1e4] range.</param>
        /// <param name="length">The length of each string. Must be within the [1,20] range.</param>
        /// <param name="characters">A string that contains the set of characters that are allowed to occur in the random strings. The maximum number of characters is 80.</param>
        /// <param name="replacement">Specifies whether the random strings should be picked with replacement.</param>
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<SimpleGenerationInfo<string>> GenerateStringsAsync(
            this IRandomOrgClient client, int count, int length, string characters, bool replacement)
        {
            return client.GenerateStringsAsync(count, length, characters, replacement, CancellationToken.None);
        }

        /// <summary>Generates version 4 true random UUIDs in accordance with section 4.4 of RFC 4122 as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random UUIDs to generate. Must be within the [1,1e3] range.</param>
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<SimpleGenerationInfo<Guid>> GenerateUuidsAsync(
            this IRandomOrgClient client, int count)
        {
            return client.GenerateUuidsAsync(count, CancellationToken.None);
        }

        /// <summary>Generates BLOBs containing true random data as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random blobs to generate. Must be within the [1,100] range.</param>
        /// <param name="size">The size of each blob, measured in bits. Must be within the [1,1048576] range and must be divisible by 8. The total size of all blobs requested must not exceed 1048576 bits.</param>
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<SimpleGenerationInfo<byte[]>> GenerateBlobsAsync(
            this IRandomOrgClient client, int count, int size)
        {
            return client.GenerateBlobsAsync(count, size, CancellationToken.None);
        }

        /// <summary>Generates true random integers within a user-defined range with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random integers to generate. Must be within the [1,1e4] range.</param>
        /// <param name="minimum">The lower boundary for the range from which the random numbers will be picked. Must be within the [-1e9,1e9] range.</param>
        /// <param name="maximum">The upper boundary for the range from which the random numbers will be picked. Must be within the [-1e9,1e9] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<SignedGenerationInfo<SignedIntegersRandom, int>> GenerateSignedIntegersAsync(
            this IRandomOrgClient client, int count, int minimum, int maximum, bool replacement)
        {
            return client.GenerateSignedIntegersAsync(count, minimum, maximum, replacement, null, CancellationToken.None);
        }

        /// <summary>Generates true random integers within a user-defined range with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random integers to generate. Must be within the [1,1e4] range.</param>
        /// <param name="minimum">The lower boundary for the range from which the random numbers will be picked. Must be within the [-1e9,1e9] range.</param>
        /// <param name="maximum">The upper boundary for the range from which the random numbers will be picked. Must be within the [-1e9,1e9] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<SignedGenerationInfo<SignedIntegersRandom, int>> GenerateSignedIntegersAsync(
            this IRandomOrgClient client, int count, int minimum, int maximum, bool replacement, CancellationToken cancellationToken)
        {
            return client.GenerateSignedIntegersAsync(count, minimum, maximum, replacement, null, cancellationToken);
        }

        /// <summary>Generates true random integers within a user-defined range with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random integers to generate. Must be within the [1,1e4] range.</param>
        /// <param name="minimum">The lower boundary for the range from which the random numbers will be picked. Must be within the [-1e9,1e9] range.</param>
        /// <param name="maximum">The upper boundary for the range from which the random numbers will be picked. Must be within the [-1e9,1e9] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1e3.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<SignedGenerationInfo<SignedIntegersRandom, int>> GenerateSignedIntegersAsync(
            this IRandomOrgClient client, int count, int minimum, int maximum, bool replacement, string userData)
        {
            return client.GenerateSignedIntegersAsync(count, minimum, maximum, replacement, userData, CancellationToken.None);
        }

        /// <summary>Generates true random decimal fractions from a uniform distribution across the [0,1] interval with a user-defined number of decimal places with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random decimal fractions to generate. Must be within the [1,1e4] range.</param>
        /// <param name="decimalPlaces">The number of decimal places to use. Must be within the [1,20] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<SignedGenerationInfo<SignedDecimalFractionsRandom, decimal>> GenerateSignedDecimalFractionsAsync(
            this IRandomOrgClient client, int count, int decimalPlaces, bool replacement)
        {
            return client.GenerateSignedDecimalFractionsAsync(count, decimalPlaces, replacement, null, CancellationToken.None);
        }

        /// <summary>Generates true random decimal fractions from a uniform distribution across the [0,1] interval with a user-defined number of decimal places with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random decimal fractions to generate. Must be within the [1,1e4] range.</param>
        /// <param name="decimalPlaces">The number of decimal places to use. Must be within the [1,20] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<SignedGenerationInfo<SignedDecimalFractionsRandom, decimal>> GenerateSignedDecimalFractionsAsync(
            this IRandomOrgClient client, int count, int decimalPlaces, bool replacement, CancellationToken cancellationToken)
        {
            return client.GenerateSignedDecimalFractionsAsync(count, decimalPlaces, replacement, null, cancellationToken);
        }

        /// <summary>Generates true random decimal fractions from a uniform distribution across the [0,1] interval with a user-defined number of decimal places with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random decimal fractions to generate. Must be within the [1,1e4] range.</param>
        /// <param name="decimalPlaces">The number of decimal places to use. Must be within the [1,20] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1e3.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<SignedGenerationInfo<SignedDecimalFractionsRandom, decimal>> GenerateSignedDecimalFractionsAsync(
            this IRandomOrgClient client, int count, int decimalPlaces, bool replacement, string userData)
        {
            return client.GenerateSignedDecimalFractionsAsync(count, decimalPlaces, replacement, userData, CancellationToken.None);
        }

        /// <summary>Generates true random numbers from a Gaussian distribution with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random numbers to generate. Must be within the [1,1e4] range.</param>
        /// <param name="mean">The distribution's mean. Must be within the [-1e6,1e6] range.</param>
        /// <param name="standardDeviation">The distribution's standard deviation. Must be within the [-1e6,1e6] range.</param>
        /// <param name="significantDigits">The number of significant digits to use. Must be within the [2,20] range.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<SignedGenerationInfo<SignedGaussiansRandom, decimal>> GenerateSignedGaussiansAsync(
            this IRandomOrgClient client, int count, decimal mean, decimal standardDeviation, int significantDigits)
        {
            return client.GenerateSignedGaussiansAsync(count, mean, standardDeviation, significantDigits, null, CancellationToken.None);
        }

        /// <summary>Generates true random numbers from a Gaussian distribution with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random numbers to generate. Must be within the [1,1e4] range.</param>
        /// <param name="mean">The distribution's mean. Must be within the [-1e6,1e6] range.</param>
        /// <param name="standardDeviation">The distribution's standard deviation. Must be within the [-1e6,1e6] range.</param>
        /// <param name="significantDigits">The number of significant digits to use. Must be within the [2,20] range.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<SignedGenerationInfo<SignedGaussiansRandom, decimal>> GenerateSignedGaussiansAsync(
            this IRandomOrgClient client, int count, decimal mean, decimal standardDeviation, int significantDigits, CancellationToken cancellationToken)
        {
            return client.GenerateSignedGaussiansAsync(count, mean, standardDeviation, significantDigits, null, cancellationToken);
        }

        /// <summary>Generates true random numbers from a Gaussian distribution with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random numbers to generate. Must be within the [1,1e4] range.</param>
        /// <param name="mean">The distribution's mean. Must be within the [-1e6,1e6] range.</param>
        /// <param name="standardDeviation">The distribution's standard deviation. Must be within the [-1e6,1e6] range.</param>
        /// <param name="significantDigits">The number of significant digits to use. Must be within the [2,20] range.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1e3.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<SignedGenerationInfo<SignedGaussiansRandom, decimal>> GenerateSignedGaussiansAsync(
            this IRandomOrgClient client, int count, decimal mean, decimal standardDeviation, int significantDigits, string userData)
        {
            return client.GenerateSignedGaussiansAsync(count, mean, standardDeviation, significantDigits, userData, CancellationToken.None);
        }

        /// <summary>Generates true random strings with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random strings to generate. Must be within the [1,1e4] range.</param>
        /// <param name="length">The length of each string. Must be within the [1,20] range.</param>
        /// <param name="characters">A string that contains the set of characters that are allowed to occur in the random strings. The maximum number of characters is 80.</param>
        /// <param name="replacement">Specifies whether the random strings should be picked with replacement.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<SignedGenerationInfo<SignedStringsRandom, string>> GenerateSignedStringsAsync(
            this IRandomOrgClient client, int count, int length, string characters, bool replacement)
        {
            return client.GenerateSignedStringsAsync(count, length, characters, replacement, null, CancellationToken.None);
        }

        /// <summary>Generates true random strings with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random strings to generate. Must be within the [1,1e4] range.</param>
        /// <param name="length">The length of each string. Must be within the [1,20] range.</param>
        /// <param name="characters">A string that contains the set of characters that are allowed to occur in the random strings. The maximum number of characters is 80.</param>
        /// <param name="replacement">Specifies whether the random strings should be picked with replacement.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<SignedGenerationInfo<SignedStringsRandom, string>> GenerateSignedStringsAsync(
            this IRandomOrgClient client, int count, int length, string characters, bool replacement, CancellationToken cancellationToken)
        {
            return client.GenerateSignedStringsAsync(count, length, characters, replacement, null, cancellationToken);
        }

        /// <summary>Generates true random strings with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random strings to generate. Must be within the [1,1e4] range.</param>
        /// <param name="length">The length of each string. Must be within the [1,20] range.</param>
        /// <param name="characters">A string that contains the set of characters that are allowed to occur in the random strings. The maximum number of characters is 80.</param>
        /// <param name="replacement">Specifies whether the random strings should be picked with replacement.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1e3.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<SignedGenerationInfo<SignedStringsRandom, string>> GenerateSignedStringsAsync(
            this IRandomOrgClient client, int count, int length, string characters, bool replacement, string userData)
        {
            return client.GenerateSignedStringsAsync(count, length, characters, replacement, userData, CancellationToken.None);
        }

        /// <summary>Generates version 4 true random UUIDs in accordance with section 4.4 of RFC 4122 with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random UUIDs to generate. Must be within the [1,1e3] range.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<SignedGenerationInfo<SignedUuidsRandom, Guid>> GenerateSignedUuidsAsync(
            this IRandomOrgClient client, int count)
        {
            return client.GenerateSignedUuidsAsync(count, null, CancellationToken.None);
        }

        /// <summary>Generates version 4 true random UUIDs in accordance with section 4.4 of RFC 4122 with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random UUIDs to generate. Must be within the [1,1e3] range.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<SignedGenerationInfo<SignedUuidsRandom, Guid>> GenerateSignedUuidsAsync(
            this IRandomOrgClient client, int count, CancellationToken cancellationToken)
        {
            return client.GenerateSignedUuidsAsync(count, null, cancellationToken);
        }

        /// <summary>Generates version 4 true random UUIDs in accordance with section 4.4 of RFC 4122 with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random UUIDs to generate. Must be within the [1,1e3] range.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1e3.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<SignedGenerationInfo<SignedUuidsRandom, Guid>> GenerateSignedUuidsAsync(
            this IRandomOrgClient client, int count, string userData)
        {
            return client.GenerateSignedUuidsAsync(count, userData, CancellationToken.None);
        }

        /// <summary>Generates BLOBs containing true random data with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random blobs to generate. Must be within the [1,100] range.</param>
        /// <param name="size">The size of each blob, measured in bits. Must be within the [1,1048576] range and must be divisible by 8. The total size of all blobs requested must not exceed 1048576 bits.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<SignedGenerationInfo<SignedBlobsRandom, byte[]>> GenerateSignedBlobsAsync(
            this IRandomOrgClient client, int count, int size)
        {
            return client.GenerateSignedBlobsAsync(count, size, null, CancellationToken.None);
        }

        /// <summary>Generates BLOBs containing true random data with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random blobs to generate. Must be within the [1,100] range.</param>
        /// <param name="size">The size of each blob, measured in bits. Must be within the [1,1048576] range and must be divisible by 8. The total size of all blobs requested must not exceed 1048576 bits.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<SignedGenerationInfo<SignedBlobsRandom, byte[]>> GenerateSignedBlobsAsync(
            this IRandomOrgClient client, int count, int size, CancellationToken cancellationToken)
        {
            return client.GenerateSignedBlobsAsync(count, size, null, cancellationToken);
        }

        /// <summary>Generates BLOBs containing true random data with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random blobs to generate. Must be within the [1,100] range.</param>
        /// <param name="size">The size of each blob, measured in bits. Must be within the [1,1048576] range and must be divisible by 8. The total size of all blobs requested must not exceed 1048576 bits.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1e3.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<SignedGenerationInfo<SignedBlobsRandom, byte[]>> GenerateSignedBlobsAsync(
            this IRandomOrgClient client, int count, int size, string userData)
        {
            return client.GenerateSignedBlobsAsync(count, size, userData, CancellationToken.None);
        }

        /// <summary>Retrieves previously generated signed results (which are stored for 24 hours) as an asynchronous operation.</summary>
        /// <typeparam name="TRandom">The type of random data container.</typeparam>
        /// <typeparam name="TValue">The type of random object.</typeparam>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="serialNumber">The integer containing the serial number associated with this random information.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<SignedGenerationInfo<TRandom, TValue>> GetResultAsync<TRandom, TValue>(
            this IRandomOrgClient client, long serialNumber)
            where TRandom : SignedRandom<TValue>
        {
            return client.GetResultAsync<TRandom, TValue>(serialNumber, CancellationToken.None);
        }

        /// <summary>Verifies the signature of signed random objects and associated data.</summary>
        /// <typeparam name="T">The type of random object.</typeparam>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="random">The signed random objects and associated data.</param>
        /// <param name="signature">The signature from the same response that the random data originates from.</param>
        /// <returns>A value, indicating if the random objects are authentic.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<bool> VerifySignatureAsync<T>(
            this IRandomOrgClient client, SignedRandom<T> random, byte[] signature)
        {
            return client.VerifySignatureAsync(random, signature, CancellationToken.None);
        }
    }
}