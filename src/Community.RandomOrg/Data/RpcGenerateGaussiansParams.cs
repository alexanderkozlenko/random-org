using Community.RandomOrg.Converters;
using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal sealed class RpcGenerateGaussiansParams : RpcGenerateParams
    {
        [JsonProperty("mean")]
        [JsonConverter(typeof(RandomDecimalConverter))]
        public decimal Mean { get; set; }

        [JsonProperty("standardDeviation")]
        [JsonConverter(typeof(RandomDecimalConverter))]
        public decimal StandardDeviation { get; set; }

        [JsonProperty("significantDigits")]
        public long SignificantDigits { get; set; }
    }
}