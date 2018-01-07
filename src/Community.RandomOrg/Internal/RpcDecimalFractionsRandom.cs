using System.ComponentModel;
using Newtonsoft.Json;

namespace Community.RandomOrg.Internal
{
    internal sealed class RpcDecimalFractionsRandom : RpcSignedRandom<decimal>
    {
        [JsonProperty("n", Required = Required.Always)]
        public int Count
        {
            get;
            set;
        }

        [JsonProperty("decimalPlaces", Required = Required.Always)]
        public int DecimalPlaces
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