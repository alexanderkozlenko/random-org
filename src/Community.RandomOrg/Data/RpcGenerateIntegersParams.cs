using System.ComponentModel;
using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal sealed class RpcGenerateIntegersParams : RpcRandomParams
    {
        [JsonProperty("min")]
        public long Minimum { get; set; }

        [JsonProperty("max")]
        public long Maximum { get; set; }

        [JsonProperty("replacement", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue(true)]
        public bool Replacement { get; set; }
    }
}