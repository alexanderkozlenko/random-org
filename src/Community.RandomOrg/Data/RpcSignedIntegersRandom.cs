using System.ComponentModel;
using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal sealed class RpcSignedIntegersRandom : RpcSignedRandom<int>
    {
        [JsonProperty("n", Required = Required.Always)]
        public long Count
        {
            get;
            set;
        }

        [JsonProperty("min", Required = Required.Always)]
        public long Minimum
        {
            get;
            set;
        }

        [JsonProperty("max", Required = Required.Always)]
        public long Maximum
        {
            get;
            set;
        }

        [JsonProperty("replacement", Required = Required.Always)]
        [DefaultValue(true)]
        public bool Replacement
        {
            get;
            set;
        }

        [JsonProperty("base", Required = Required.Always)]
        public long Base
        {
            get;
            set;
        }
    }
}