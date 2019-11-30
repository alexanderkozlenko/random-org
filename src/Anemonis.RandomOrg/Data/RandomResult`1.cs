// © Alexander Kozlenko. Licensed under the MIT License.

using System;

namespace Anemonis.RandomOrg.Data
{
    /// <summary>Encapsulates random data generation result.</summary>
    /// <typeparam name="T">The type of random object.</typeparam>
    public sealed class RandomResult<T> : RandomResultObject<Random<T>, T>
    {
        internal RandomResult(Random<T> random, long bitsUsed, long bitsLeft, long requestsLeft, TimeSpan advisoryDelay)
            : base(random, bitsUsed, bitsLeft, requestsLeft, advisoryDelay)
        {
        }
    }
}
