using Community.RandomOrg.Converters;
using Newtonsoft.Json;

namespace Community.RandomOrg.Internal
{
    internal sealed class RpcGaussiansRandom : RpcSignedRandom<decimal>
    {
        [JsonProperty("n", Required = Required.Always)]
        public int Count
        {
            get;
            set;
        }

        [JsonProperty("mean", Required = Required.Always)]
        [JsonConverter(typeof(RandomDecimalConverter))]
        public decimal Mean
        {
            get;
            set;
        }

        [JsonProperty("standardDeviation", Required = Required.Always)]
        [JsonConverter(typeof(RandomDecimalConverter))]
        public decimal StandardDeviation
        {
            get;
            set;
        }

        [JsonProperty("significantDigits", Required = Required.Always)]
        public int SignificantDigits
        {
            get;
            set;
        }
    }
}