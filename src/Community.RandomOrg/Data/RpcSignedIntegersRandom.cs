using System.ComponentModel;
using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal sealed class RpcSignedIntegersRandom : RpcSignedRandom<int>
    {
        [JsonProperty("min")]
        public long Minimum { get; set; }

        [JsonProperty("max")]
        public long Maximum { get; set; }

        [JsonProperty("replacement")]
        [DefaultValue(true)]
        public bool Replacement { get; set; }

        [JsonProperty("base")]
        public long Base { get; set; }
    }
}