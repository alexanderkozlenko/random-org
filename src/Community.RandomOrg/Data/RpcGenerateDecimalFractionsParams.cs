using System.ComponentModel;
using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal sealed class RpcGenerateDecimalFractionsParams : RpcRandomParams
    {
        [JsonProperty("decimalPlaces")]
        public long DecimalPlaces { get; set; }

        [JsonProperty("replacement", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue(true)]
        public bool Replacement { get; set; }
    }
}