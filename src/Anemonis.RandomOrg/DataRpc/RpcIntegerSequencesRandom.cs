// © Alexander Kozlenko. Licensed under the MIT License.

using System.Collections.Generic;

using Newtonsoft.Json;

namespace Anemonis.RandomOrg.DataRpc
{
    internal sealed class RpcIntegerSequencesRandom : RpcSignedRandom<IReadOnlyList<int>>
    {
        [JsonProperty("n", Required = Required.Always)]
        public int Count
        {
            get;
            set;
        }

        [JsonProperty("length", Required = Required.Always)]
        public IReadOnlyList<int> Lengths
        {
            get;
            set;
        }

        [JsonProperty("min", Required = Required.Always)]
        public IReadOnlyList<int> Minimums
        {
            get;
            set;
        }

        [JsonProperty("max", Required = Required.Always)]
        public IReadOnlyList<int> Maximums
        {
            get;
            set;
        }

        [JsonProperty("replacement", Required = Required.Always)]
        public IReadOnlyList<bool> Replacements
        {
            get;
            set;
        }

        [JsonProperty("base", Required = Required.Always)]
        public IReadOnlyList<int> Bases
        {
            get;
            set;
        }
    }
}
