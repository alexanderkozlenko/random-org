// © Alexander Kozlenko. Licensed under the MIT License.

using System.Collections.Generic;

namespace Community.RandomOrg.Internal
{
    internal static class EmptyDictionary<TKey, TValue>
    {
        public static readonly Dictionary<TKey, TValue> Instance = new Dictionary<TKey, TValue>(0);
    }
}