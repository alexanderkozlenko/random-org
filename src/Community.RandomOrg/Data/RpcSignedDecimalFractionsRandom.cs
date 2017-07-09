using System.ComponentModel;
using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal sealed class RpcSignedDecimalFractionsRandom : RpcSignedRandom<decimal>
    {
        [JsonProperty("decimalPlaces")]
        public long DecimalPlaces { get; set; }

        [JsonProperty("replacement")]
        [DefaultValue(true)]
        public bool Replacement { get; set; }
    }
}