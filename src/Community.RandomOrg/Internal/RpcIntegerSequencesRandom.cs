using System.Collections.Generic;
using Newtonsoft.Json;

namespace Community.RandomOrg.Internal
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
        public int[] Lengths
        {
            get;
            set;
        }

        [JsonProperty("min", Required = Required.Always)]
        public int[] Minimums
        {
            get;
            set;
        }

        [JsonProperty("max", Required = Required.Always)]
        public int[] Maximums
        {
            get;
            set;
        }

        [JsonProperty("replacement", Required = Required.Always)]
        public bool[] Replacements
        {
            get;
            set;
        }

        [JsonProperty("base", Required = Required.Always)]
        public int[] Bases
        {
            get;
            set;
        }
    }
}