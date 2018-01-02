using System.ComponentModel;
using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal sealed class RpcSignedDecimalFractionsRandom : RpcSignedRandom<decimal>
    {
        [JsonProperty("n", Required = Required.Always)]
        public long Count
        {
            get;
            set;
        }

        [JsonProperty("decimalPlaces", Required = Required.Always)]
        public long DecimalPlaces
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
    }
}