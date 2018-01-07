using System;
using System.Collections.Generic;
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
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<RandomUsage> GetUsageAsync(
            this RandomOrgClient client)
        {
            return client.GetUsageAsync(CancellationToken.None);
        }

        /// <summary>Generates true random integers within a user-defined range as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random integers to generate. Must be within the [1,10000] range.</param>
        /// <param name="minimum">The lower boundary for the range from which the random numbers will be picked. Must be within the [-1000000000,1000000000] range.</param>
        /// <param name="maximum">The upper boundary for the range from which the random numbers will be picked. Must be within the [-1000000000,1000000000] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <returns>A <see cref="RandomResult{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="minimum" />, or <paramref name="maximum" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<RandomResult<int>> GenerateIntegersAsync(
            this RandomOrgClient client, int count, int minimum, int maximum, bool replacement)
        {
            return client.GenerateIntegersAsync(count, minimum, maximum, replacement, CancellationToken.None);
        }

        /// <summary>Generates true random integers within a user-defined range as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="counts">An array specifying the lengths of the requested sequences. Each value must be within the [1,10000] range and the total sum of all the lengths must be in the [1,10000] range. Up to 10 sequences can be requested.</param>
        /// <param name="minimums">An array specifying the lower boundaries for the requested sequences. Each value must be within the [-1000000000,1000000000] range.</param>
        /// <param name="maximums">An array specifying the upper boundaries for the requested sequences. Each value must be within the [-1000000000,1000000000] range.</param>
        /// <param name="replacements">An array specifying for each requested sequence whether the random numbers in that seqeuence should be picked with replacement.</param>
        /// <returns>A <see cref="RandomResult{T}" /> instance.</returns>
        /// <exception cref="ArgumentException">Counts of argument arrays are different, sequences count is greater than 10, or total count is outside the [1,10000] range.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="counts" />, <paramref name="minimums" />, <paramref name="maximums" />, or <paramref name="replacements" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">One of the values from the arguments <paramref name="counts" />, <paramref name="minimums" />, or <paramref name="maximums" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<RandomResult<int[]>> GenerateIntegerSequencesAsync(
            this RandomOrgClient client, IReadOnlyList<int> counts, IReadOnlyList<int> minimums, IReadOnlyList<int> maximums, IReadOnlyList<bool> replacements)
        {
            return client.GenerateIntegerSequencesAsync(counts, minimums, maximums, replacements, CancellationToken.None);
        }

        /// <summary>Generates true random decimal fractions from a uniform distribution across the [0,1] interval with a user-defined number of decimal places as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random decimal fractions to generate. Must be within the [1,10000] range.</param>
        /// <param name="decimalPlaces">The number of decimal places to use. Must be within the [1,20] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <returns>A <see cref="RandomResult{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="decimalPlaces" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<RandomResult<decimal>> GenerateDecimalFractionsAsync(
            this RandomOrgClient client, int count, int decimalPlaces, bool replacement)
        {
            return client.GenerateDecimalFractionsAsync(count, decimalPlaces, replacement, CancellationToken.None);
        }

        /// <summary>Generates true random numbers from a Gaussian distribution as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random numbers to generate. Must be within the [1,10000] range.</param>
        /// <param name="mean">The distribution's mean. Must be within the [-1000000,1000000] range.</param>
        /// <param name="standardDeviation">The distribution's standard deviation. Must be within the [-1000000,1000000] range.</param>
        /// <param name="significantDigits">The number of significant digits to use. Must be within the [2,20] range.</param>
        /// <returns>A <see cref="RandomResult{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="mean" />, <paramref name="standardDeviation" />, or <paramref name="significantDigits" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<RandomResult<decimal>> GenerateGaussiansAsync(
            this RandomOrgClient client, int count, decimal mean, decimal standardDeviation, int significantDigits)
        {
            return client.GenerateGaussiansAsync(count, mean, standardDeviation, significantDigits, CancellationToken.None);
        }

        /// <summary>Generates true random strings as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random strings to generate. Must be within the [1,10000] range.</param>
        /// <param name="length">The length of each string. Must be within the [1,20] range.</param>
        /// <param name="characters">A string that contains the set of characters that are allowed to occur in the random strings. The maximum number of characters is 80.</param>
        /// <param name="replacement">Specifies whether the random strings should be picked with replacement.</param>
        /// <returns>A <see cref="RandomResult{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="length" /> is outside the allowable range of values.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="characters" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="characters" /> contains invalid number of characters .</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<RandomResult<string>> GenerateStringsAsync(
            this RandomOrgClient client, int count, int length, string characters, bool replacement)
        {
            return client.GenerateStringsAsync(count, length, characters, replacement, CancellationToken.None);
        }

        /// <summary>Generates version 4 true random UUIDs in accordance with section 4.4 of RFC 4122 as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random UUIDs to generate. Must be within the [1,1000] range.</param>
        /// <returns>A <see cref="RandomResult{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<RandomResult<Guid>> GenerateUuidsAsync(
            this RandomOrgClient client, int count)
        {
            return client.GenerateUuidsAsync(count, CancellationToken.None);
        }

        /// <summary>Generates BLOBs containing true random data as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random blobs to generate. Must be within the [1,100] range.</param>
        /// <param name="size">The size of each blob, measured in bits. Must be within the [1,1048576] range and must be divisible by 8. The total size of all blobs requested must not exceed 1048576 bits.</param>
        /// <returns>A <see cref="RandomResult{T}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="size" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<RandomResult<byte[]>> GenerateBlobsAsync(
            this RandomOrgClient client, int count, int size)
        {
            return client.GenerateBlobsAsync(count, size, CancellationToken.None);
        }

        /// <summary>Generates true random integers within a user-defined range with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random integers to generate. Must be within the [1,10000] range.</param>
        /// <param name="minimum">The lower boundary for the range from which the random numbers will be picked. Must be within the [-1000000000,1000000000] range.</param>
        /// <param name="maximum">The upper boundary for the range from which the random numbers will be picked. Must be within the [-1000000000,1000000000] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <returns>A <see cref="SignedRandomResult{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="minimum" />, or <paramref name="maximum" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<SignedRandomResult<int, IntegerParameters>> GenerateSignedIntegersAsync(
            this RandomOrgClient client, int count, int minimum, int maximum, bool replacement)
        {
            return client.GenerateSignedIntegersAsync(count, minimum, maximum, replacement, null, CancellationToken.None);
        }

        /// <summary>Generates true random integers within a user-defined range with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random integers to generate. Must be within the [1,10000] range.</param>
        /// <param name="minimum">The lower boundary for the range from which the random numbers will be picked. Must be within the [-1000000000,1000000000] range.</param>
        /// <param name="maximum">The upper boundary for the range from which the random numbers will be picked. Must be within the [-1000000000,1000000000] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A <see cref="SignedRandomResult{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="minimum" />, or <paramref name="maximum" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<SignedRandomResult<int, IntegerParameters>> GenerateSignedIntegersAsync(
            this RandomOrgClient client, int count, int minimum, int maximum, bool replacement, CancellationToken cancellationToken)
        {
            return client.GenerateSignedIntegersAsync(count, minimum, maximum, replacement, null, cancellationToken);
        }

        /// <summary>Generates true random integers within a user-defined range with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random integers to generate. Must be within the [1,10000] range.</param>
        /// <param name="minimum">The lower boundary for the range from which the random numbers will be picked. Must be within the [-1000000000,1000000000] range.</param>
        /// <param name="maximum">The upper boundary for the range from which the random numbers will be picked. Must be within the [-1000000000,1000000000] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1000.</param>
        /// <returns>A <see cref="SignedRandomResult{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="minimum" />, or <paramref name="maximum" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<SignedRandomResult<int, IntegerParameters>> GenerateSignedIntegersAsync(
            this RandomOrgClient client, int count, int minimum, int maximum, bool replacement, string userData)
        {
            return client.GenerateSignedIntegersAsync(count, minimum, maximum, replacement, userData, CancellationToken.None);
        }

        /// <summary>Generates true random integers within a user-defined range with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="counts">An array specifying the lengths of the requested sequences. Each value must be within the [1,10000] range and the total sum of all the lengths must be in the [1,10000] range. Up to 10 sequences can be requested.</param>
        /// <param name="minimums">An array specifying the lower boundaries for the requested sequences. Each value must be within the [-1000000000,1000000000] range.</param>
        /// <param name="maximums">An array specifying the upper boundaries for the requested sequences. Each value must be within the [-1000000000,1000000000] range.</param>
        /// <param name="replacements">An array specifying for each requested sequence whether the random numbers in that seqeuence should be picked with replacement.</param>
        /// <returns>A <see cref="RandomResult{T}" /> instance.</returns>
        /// <exception cref="ArgumentException">Counts of argument arrays are different, sequences count is greater than 10, or total count is outside the [1,10000] range.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="counts" />, <paramref name="minimums" />, <paramref name="maximums" />, or <paramref name="replacements" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">One of the values from the arguments <paramref name="counts" />, <paramref name="minimums" />, or <paramref name="maximums" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<SignedRandomResult<int[], IntegerSequenceParameters>> GenerateSignedIntegerSequencesAsync(
            this RandomOrgClient client, IReadOnlyList<int> counts, IReadOnlyList<int> minimums, IReadOnlyList<int> maximums, IReadOnlyList<bool> replacements)
        {
            return client.GenerateSignedIntegerSequencesAsync(counts, minimums, maximums, replacements, null, CancellationToken.None);
        }

        /// <summary>Generates true random integers within a user-defined range with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
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
        public static Task<SignedRandomResult<int[], IntegerSequenceParameters>> GenerateSignedIntegerSequencesAsync(
            this RandomOrgClient client, IReadOnlyList<int> counts, IReadOnlyList<int> minimums, IReadOnlyList<int> maximums, IReadOnlyList<bool> replacements, CancellationToken cancellationToken)
        {
            return client.GenerateSignedIntegerSequencesAsync(counts, minimums, maximums, replacements, null, cancellationToken);
        }

        /// <summary>Generates true random integers within a user-defined range with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="counts">An array specifying the lengths of the requested sequences. Each value must be within the [1,10000] range and the total sum of all the lengths must be in the [1,10000] range. Up to 10 sequences can be requested.</param>
        /// <param name="minimums">An array specifying the lower boundaries for the requested sequences. Each value must be within the [-1000000000,1000000000] range.</param>
        /// <param name="maximums">An array specifying the upper boundaries for the requested sequences. Each value must be within the [-1000000000,1000000000] range.</param>
        /// <param name="replacements">An array specifying for each requested sequence whether the random numbers in that seqeuence should be picked with replacement.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1000.</param>
        /// <returns>A <see cref="RandomResult{T}" /> instance.</returns>
        /// <exception cref="ArgumentException">Counts of argument arrays are different, sequences count is greater than 10, or total count is outside the [1,10000] range.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="counts" />, <paramref name="minimums" />, <paramref name="maximums" />, or <paramref name="replacements" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">One of the values from the arguments <paramref name="counts" />, <paramref name="minimums" />, or <paramref name="maximums" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<SignedRandomResult<int[], IntegerSequenceParameters>> GenerateSignedIntegerSequencesAsync(
            this RandomOrgClient client, IReadOnlyList<int> counts, IReadOnlyList<int> minimums, IReadOnlyList<int> maximums, IReadOnlyList<bool> replacements, string userData)
        {
            return client.GenerateSignedIntegerSequencesAsync(counts, minimums, maximums, replacements, userData, CancellationToken.None);
        }

        /// <summary>Generates true random decimal fractions from a uniform distribution across the [0,1] interval with a user-defined number of decimal places with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random decimal fractions to generate. Must be within the [1,10000] range.</param>
        /// <param name="decimalPlaces">The number of decimal places to use. Must be within the [1,20] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <returns>A <see cref="SignedRandomResult{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="decimalPlaces" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<SignedRandomResult<decimal, DecimalFractionParameters>> GenerateSignedDecimalFractionsAsync(
            this RandomOrgClient client, int count, int decimalPlaces, bool replacement)
        {
            return client.GenerateSignedDecimalFractionsAsync(count, decimalPlaces, replacement, null, CancellationToken.None);
        }

        /// <summary>Generates true random decimal fractions from a uniform distribution across the [0,1] interval with a user-defined number of decimal places with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random decimal fractions to generate. Must be within the [1,10000] range.</param>
        /// <param name="decimalPlaces">The number of decimal places to use. Must be within the [1,20] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A <see cref="SignedRandomResult{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="decimalPlaces" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<SignedRandomResult<decimal, DecimalFractionParameters>> GenerateSignedDecimalFractionsAsync(
            this RandomOrgClient client, int count, int decimalPlaces, bool replacement, CancellationToken cancellationToken)
        {
            return client.GenerateSignedDecimalFractionsAsync(count, decimalPlaces, replacement, null, cancellationToken);
        }

        /// <summary>Generates true random decimal fractions from a uniform distribution across the [0,1] interval with a user-defined number of decimal places with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random decimal fractions to generate. Must be within the [1,10000] range.</param>
        /// <param name="decimalPlaces">The number of decimal places to use. Must be within the [1,20] range.</param>
        /// <param name="replacement">Specifies whether the random numbers should be picked with replacement.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1000.</param>
        /// <returns>A <see cref="SignedRandomResult{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="decimalPlaces" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<SignedRandomResult<decimal, DecimalFractionParameters>> GenerateSignedDecimalFractionsAsync(
            this RandomOrgClient client, int count, int decimalPlaces, bool replacement, string userData)
        {
            return client.GenerateSignedDecimalFractionsAsync(count, decimalPlaces, replacement, userData, CancellationToken.None);
        }

        /// <summary>Generates true random numbers from a Gaussian distribution with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random numbers to generate. Must be within the [1,10000] range.</param>
        /// <param name="mean">The distribution's mean. Must be within the [-1000000,1000000] range.</param>
        /// <param name="standardDeviation">The distribution's standard deviation. Must be within the [-1000000,1000000] range.</param>
        /// <param name="significantDigits">The number of significant digits to use. Must be within the [2,20] range.</param>
        /// <returns>A <see cref="SignedRandomResult{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="mean" />, <paramref name="standardDeviation" />, or <paramref name="significantDigits" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<SignedRandomResult<decimal, GaussianParameters>> GenerateSignedGaussiansAsync(
            this RandomOrgClient client, int count, decimal mean, decimal standardDeviation, int significantDigits)
        {
            return client.GenerateSignedGaussiansAsync(count, mean, standardDeviation, significantDigits, null, CancellationToken.None);
        }

        /// <summary>Generates true random numbers from a Gaussian distribution with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random numbers to generate. Must be within the [1,10000] range.</param>
        /// <param name="mean">The distribution's mean. Must be within the [-1000000,1000000] range.</param>
        /// <param name="standardDeviation">The distribution's standard deviation. Must be within the [-1000000,1000000] range.</param>
        /// <param name="significantDigits">The number of significant digits to use. Must be within the [2,20] range.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A <see cref="SignedRandomResult{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="mean" />, <paramref name="standardDeviation" />, or <paramref name="significantDigits" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<SignedRandomResult<decimal, GaussianParameters>> GenerateSignedGaussiansAsync(
            this RandomOrgClient client, int count, decimal mean, decimal standardDeviation, int significantDigits, CancellationToken cancellationToken)
        {
            return client.GenerateSignedGaussiansAsync(count, mean, standardDeviation, significantDigits, null, cancellationToken);
        }

        /// <summary>Generates true random numbers from a Gaussian distribution with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random numbers to generate. Must be within the [1,10000] range.</param>
        /// <param name="mean">The distribution's mean. Must be within the [-1000000,1000000] range.</param>
        /// <param name="standardDeviation">The distribution's standard deviation. Must be within the [-1000000,1000000] range.</param>
        /// <param name="significantDigits">The number of significant digits to use. Must be within the [2,20] range.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1000.</param>
        /// <returns>A <see cref="SignedRandomResult{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" />, <paramref name="mean" />, <paramref name="standardDeviation" />, or <paramref name="significantDigits" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<SignedRandomResult<decimal, GaussianParameters>> GenerateSignedGaussiansAsync(
            this RandomOrgClient client, int count, decimal mean, decimal standardDeviation, int significantDigits, string userData)
        {
            return client.GenerateSignedGaussiansAsync(count, mean, standardDeviation, significantDigits, userData, CancellationToken.None);
        }

        /// <summary>Generates true random strings with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random strings to generate. Must be within the [1,10000] range.</param>
        /// <param name="length">The length of each string. Must be within the [1,20] range.</param>
        /// <param name="characters">A string that contains the set of characters that are allowed to occur in the random strings. The maximum number of characters is 80.</param>
        /// <param name="replacement">Specifies whether the random strings should be picked with replacement.</param>
        /// <returns>A <see cref="SignedRandomResult{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="length" /> is outside the allowable range of values.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="characters" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="characters" /> contains invalid number of characters .</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<SignedRandomResult<string, StringParameters>> GenerateSignedStringsAsync(
            this RandomOrgClient client, int count, int length, string characters, bool replacement)
        {
            return client.GenerateSignedStringsAsync(count, length, characters, replacement, null, CancellationToken.None);
        }

        /// <summary>Generates true random strings with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random strings to generate. Must be within the [1,10000] range.</param>
        /// <param name="length">The length of each string. Must be within the [1,20] range.</param>
        /// <param name="characters">A string that contains the set of characters that are allowed to occur in the random strings. The maximum number of characters is 80.</param>
        /// <param name="replacement">Specifies whether the random strings should be picked with replacement.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A <see cref="SignedRandomResult{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="length" /> is outside the allowable range of values.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="characters" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="characters" /> contains invalid number of characters .</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<SignedRandomResult<string, StringParameters>> GenerateSignedStringsAsync(
            this RandomOrgClient client, int count, int length, string characters, bool replacement, CancellationToken cancellationToken)
        {
            return client.GenerateSignedStringsAsync(count, length, characters, replacement, null, cancellationToken);
        }

        /// <summary>Generates true random strings with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random strings to generate. Must be within the [1,10000] range.</param>
        /// <param name="length">The length of each string. Must be within the [1,20] range.</param>
        /// <param name="characters">A string that contains the set of characters that are allowed to occur in the random strings. The maximum number of characters is 80.</param>
        /// <param name="replacement">Specifies whether the random strings should be picked with replacement.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1000.</param>
        /// <returns>A <see cref="SignedRandomResult{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="length" /> is outside the allowable range of values.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="characters" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="characters" /> contains invalid number of characters .</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<SignedRandomResult<string, StringParameters>> GenerateSignedStringsAsync(
            this RandomOrgClient client, int count, int length, string characters, bool replacement, string userData)
        {
            return client.GenerateSignedStringsAsync(count, length, characters, replacement, userData, CancellationToken.None);
        }

        /// <summary>Generates version 4 true random UUIDs in accordance with section 4.4 of RFC 4122 with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random UUIDs to generate. Must be within the [1,1000] range.</param>
        /// <returns>A <see cref="SignedRandomResult{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<SignedRandomResult<Guid, UuidParameters>> GenerateSignedUuidsAsync(
            this RandomOrgClient client, int count)
        {
            return client.GenerateSignedUuidsAsync(count, null, CancellationToken.None);
        }

        /// <summary>Generates version 4 true random UUIDs in accordance with section 4.4 of RFC 4122 with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random UUIDs to generate. Must be within the [1,1000] range.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A <see cref="SignedRandomResult{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<SignedRandomResult<Guid, UuidParameters>> GenerateSignedUuidsAsync(
            this RandomOrgClient client, int count, CancellationToken cancellationToken)
        {
            return client.GenerateSignedUuidsAsync(count, null, cancellationToken);
        }

        /// <summary>Generates version 4 true random UUIDs in accordance with section 4.4 of RFC 4122 with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random UUIDs to generate. Must be within the [1,1000] range.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1000.</param>
        /// <returns>A <see cref="SignedRandomResult{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<SignedRandomResult<Guid, UuidParameters>> GenerateSignedUuidsAsync(
            this RandomOrgClient client, int count, string userData)
        {
            return client.GenerateSignedUuidsAsync(count, userData, CancellationToken.None);
        }

        /// <summary>Generates BLOBs containing true random data with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random blobs to generate. Must be within the [1,100] range.</param>
        /// <param name="size">The size of each blob, measured in bits. Must be within the [1,1048576] range and must be divisible by 8. The total size of all blobs requested must not exceed 1048576 bits.</param>
        /// <returns>A <see cref="SignedRandomResult{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="size" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<SignedRandomResult<byte[], BlobParameters>> GenerateSignedBlobsAsync(
            this RandomOrgClient client, int count, int size)
        {
            return client.GenerateSignedBlobsAsync(count, size, null, CancellationToken.None);
        }

        /// <summary>Generates BLOBs containing true random data with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random blobs to generate. Must be within the [1,100] range.</param>
        /// <param name="size">The size of each blob, measured in bits. Must be within the [1,1048576] range and must be divisible by 8. The total size of all blobs requested must not exceed 1048576 bits.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A <see cref="SignedRandomResult{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="size" /> is outside the allowable range of values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<SignedRandomResult<byte[], BlobParameters>> GenerateSignedBlobsAsync(
            this RandomOrgClient client, int count, int size, CancellationToken cancellationToken)
        {
            return client.GenerateSignedBlobsAsync(count, size, null, cancellationToken);
        }

        /// <summary>Generates BLOBs containing true random data with signature as an asynchronous operation.</summary>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="count">How many random blobs to generate. Must be within the [1,100] range.</param>
        /// <param name="size">The size of each blob, measured in bits. Must be within the [1,1048576] range and must be divisible by 8. The total size of all blobs requested must not exceed 1048576 bits.</param>
        /// <param name="userData">The optional string that will be included in unmodified form in the signed response along with the random data. The maximum number of characters is 1000.</param>
        /// <returns>A <see cref="SignedRandomResult{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> or <paramref name="size" /> is outside the allowable range of values.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<SignedRandomResult<byte[], BlobParameters>> GenerateSignedBlobsAsync(
            this RandomOrgClient client, int count, int size, string userData)
        {
            return client.GenerateSignedBlobsAsync(count, size, userData, CancellationToken.None);
        }

        /// <summary>Verifies the signature of signed random objects and associated data.</summary>
        /// <typeparam name="TValue">The type of random object.</typeparam>
        /// <typeparam name="TParameters">The type of random parameters.</typeparam>
        /// <param name="client">The <see cref="RandomOrgClient" /> instance.</param>
        /// <param name="random">The signed random objects and associated data.</param>
        /// <param name="signature">The signature from the same response that the random data originates from.</param>
        /// <returns>A value, indicating if the random objects are authentic.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="random" /> or <paramref name="signature" /> is <see langword="null" />.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public static Task<bool> VerifySignatureAsync<TValue, TParameters>(
            this RandomOrgClient client, SignedRandom<TValue, TParameters> random, byte[] signature)
            where TParameters : RandomParameters, new()
        {
            return client.VerifySignatureAsync(random, signature, CancellationToken.None);
        }
    }
}