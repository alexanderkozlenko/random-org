using System.ComponentModel;
using Newtonsoft.Json;

namespace Community.RandomOrg.Internal
{
    internal sealed class RpcIntegerSequencesRandom : RpcSignedRandom<int[]>
    {
        [JsonProperty("n", Required = Required.Always)]
        public int[] Counts
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
        [DefaultValue(true)]
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