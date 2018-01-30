using Newtonsoft.Json;

namespace Community.RandomOrg.Internal
{
    internal sealed class RpcIntegersRandom : RpcSignedRandom<int>
    {
        [JsonProperty("n", Required = Required.Always)]
        public int Count
        {
            get;
            set;
        }

        [JsonProperty("min", Required = Required.Always)]
        public int Minimum
        {
            get;
            set;
        }

        [JsonProperty("max", Required = Required.Always)]
        public int Maximum
        {
            get;
            set;
        }

        [JsonProperty("replacement", Required = Required.Always)]
        public bool Replacement
        {
            get;
            set;
        }

        [JsonProperty("base", Required = Required.Always)]
        public int Base
        {
            get;
            set;
        }
    }
}