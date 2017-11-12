using System;
using System.Threading;
using System.Threading.Tasks;
using Community.RandomOrg.Data;

namespace Community.RandomOrg
{
    /// <summary>Defines a RANDOM.ORG service client.</summary>
    public interface IRandomOrgClient : IDisposable
    {
        /// <summary>Returns information related to the usage of a given API key as an asynchronous operation.</summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="RandomUsage" /> instance.</returns>
        Task<RandomUsage> GetUsageAsync(
            CancellationToken cancellationToken);

        /// <summary>Generates true random integers within a user-defined range as an asynchronous operation.</summary>
        /// <param name="count">How many random integers to generate. Must be within the [1,1e4] range.</param>
        /// <param name="minimum">The lower boundary for the range from which the random numbers will be picked. Must be within the [-1e9,1e9] range.</param>
        /// <param name="maximum">The upper boundary for the range from which the random numbers will be picked. Must be within the [-1e9,1e9] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        Task<SimpleGenerationInfo<int>> GenerateIntegersAsync(
            int count, int minimum, int maximum, bool replacement, CancellationToken cancellationToken);

        /// <summary>Generates true random decimal fractions from a uniform distribution across the [0,1] interval with a user-defined number of decimal places as an asynchronous operation.</summary>
        /// <param name="count">How many random decimal fractions to generate. Must be within the [1,1e4] range.</param>
        /// <param name="decimalPlaces">The number of decimal places to use. Must be within the [1,20] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        Task<SimpleGenerationInfo<decimal>> GenerateDecimalFractionsAsync(
            int count, int decimalPlaces, bool replacement, CancellationToken cancellationToken);

        /// <summary>Generates true random numbers from a Gaussian distribution as an asynchronous operation.</summary>
        /// <param name="count">How many random numbers to generate. Must be within the [1,1e4] range.</param>
        /// <param name="mean">The distribution's mean. Must be within the [-1e6,1e6] range.</param>
        /// <param name="standardDeviation">The distribution's standard deviation. Must be within the [-1e6,1e6] range.</param>
        /// <param name="significantDigits">The number of significant digits to use. Must be within the [2,20] range.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        Task<SimpleGenerationInfo<decimal>> GenerateGaussiansAsync(
            int count, decimal mean, decimal standardDeviation, int significantDigits, CancellationToken cancellationToken);

        /// <summary>Generates true random strings as an asynchronous operation.</summary>
        /// <param name="count">How many random strings to generate. Must be within the [1,1e4] range.</param>
        /// <param name="length">The length of each string. Must be within the [1,20] range.</param>
        /// <param name="characters">A string that contains the set of characters that are allowed to occur in the random strings. The maximum number of characters is 80.</param>
        /// <param name="replacement">Specifies whether the random strings should be picked with replacement.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        Task<SimpleGenerationInfo<string>> GenerateStringsAsync(
            int count, int length, string characters, bool replacement, CancellationToken cancellationToken);

        /// <summary>Generates version 4 true random UUIDs in accordance with section 4.4 of RFC 4122 as an asynchronous operation.</summary>
        /// <param name="count">How many random UUIDs to generate. Must be within the [1,1e3] range.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        Task<SimpleGenerationInfo<Guid>> GenerateUuidsAsync(
            int count, CancellationToken cancellationToken);

        /// <summary>Generates BLOBs containing true random data as an asynchronous operation.</summary>
        /// <param name="count">How many random blobs to generate. Must be within the [1,100] range.</param>
        /// <param name="size">The size of each blob, measured in bits. Must be within the [1,1048576] range and must be divisible by 8. The total size of all blobs requested must not exceed 1048576 bits.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SimpleGenerationInfo{T}" /> instance.</returns>
        Task<SimpleGenerationInfo<byte[]>> GenerateBlobsAsync(
            int count, int size, CancellationToken cancellationToken);

        /// <summary>Generates true random integers within a user-defined range with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random integers to generate. Must be within the [1,1e4] range.</param>
        /// <param name="minimum">The lower boundary for the range from which the random numbers will be picked. Must be within the [-1e9,1e9] range.</param>
        /// <param name="maximum">The upper boundary for the range from which the random numbers will be picked. Must be within the [-1e9,1e9] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1e3.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        Task<SignedGenerationInfo<SignedIntegersRandom, int>> GenerateSignedIntegersAsync(
            int count, int minimum, int maximum, bool replacement, string userData, CancellationToken cancellationToken);

        /// <summary>Generates true random decimal fractions from a uniform distribution across the [0,1] interval with a user-defined number of decimal places with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random decimal fractions to generate. Must be within the [1,1e4] range.</param>
        /// <param name="decimalPlaces">The number of decimal places to use. Must be within the [1,20] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1e3.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        Task<SignedGenerationInfo<SignedDecimalFractionsRandom, decimal>> GenerateSignedDecimalFractionsAsync(
            int count, int decimalPlaces, bool replacement, string userData, CancellationToken cancellationToken);

        /// <summary>Generates true random numbers from a Gaussian distribution with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random numbers to generate. Must be within the [1,1e4] range.</param>
        /// <param name="mean">The distribution's mean. Must be within the [-1e6,1e6] range.</param>
        /// <param name="standardDeviation">The distribution's standard deviation. Must be within the [-1e6,1e6] range.</param>
        /// <param name="significantDigits">The number of significant digits to use. Must be within the [2,20] range.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1e3.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        Task<SignedGenerationInfo<SignedGaussiansRandom, decimal>> GenerateSignedGaussiansAsync(
            int count, decimal mean, decimal standardDeviation, int significantDigits, string userData, CancellationToken cancellationToken);

        /// <summary>Generates true random strings with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random strings to generate. Must be within the [1,1e4] range.</param>
        /// <param name="length">The length of each string. Must be within the [1,20] range.</param>
        /// <param name="characters">A string that contains the set of characters that are allowed to occur in the random strings. The maximum number of characters is 80.</param>
        /// <param name="replacement">Specifies whether the random strings should be picked with replacement.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1e3.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        Task<SignedGenerationInfo<SignedStringsRandom, string>> GenerateSignedStringsAsync(
            int count, int length, string characters, bool replacement, string userData, CancellationToken cancellationToken);

        /// <summary>Generates version 4 true random UUIDs in accordance with section 4.4 of RFC 4122 with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random UUIDs to generate. Must be within the [1,1e3] range.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1e3.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        Task<SignedGenerationInfo<SignedUuidsRandom, Guid>> GenerateSignedUuidsAsync(
            int count, string userData, CancellationToken cancellationToken);

        /// <summary>Generates BLOBs containing true random data with signature as an asynchronous operation.</summary>
        /// <param name="count">How many random blobs to generate. Must be within the [1,100] range.</param>
        /// <param name="size">The size of each blob, measured in bits. Must be within the [1,1048576] range and must be divisible by 8. The total size of all blobs requested must not exceed 1048576 bits.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1e3.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        Task<SignedGenerationInfo<SignedBlobsRandom, byte[]>> GenerateSignedBlobsAsync(
            int count, int size, string userData, CancellationToken cancellationToken);

        /// <summary>Retrieves previously generated signed results (which are stored for 24 hours) as an asynchronous operation.</summary>
        /// <typeparam name="TRandom">The type of random data container.</typeparam>
        /// <typeparam name="TValue">The type of random object.</typeparam>
        /// <param name="serialNumber">The integer containing the serial number associated with this random information.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        Task<SignedGenerationInfo<TRandom, TValue>> GetResultAsync<TRandom, TValue>(
            long serialNumber, CancellationToken cancellationToken)
            where TRandom : SignedRandom<TValue>;

        /// <summary>Verifies the signature of signed random objects and associated data.</summary>
        /// <typeparam name="T">The type of random object.</typeparam>
        /// <param name="random">The signed random objects and associated data.</param>
        /// <param name="signature">The signature from the same response that the random data originates from.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A value, indicating if the random objects are authentic.</returns>
        Task<bool> VerifySignatureAsync<T>(
            SignedRandom<T> random, byte[] signature, CancellationToken cancellationToken);
    }
}