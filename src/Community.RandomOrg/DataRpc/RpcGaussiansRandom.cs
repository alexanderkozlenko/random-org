// © Alexander Kozlenko. Licensed under the MIT License.

using Newtonsoft.Json;

namespace Community.RandomOrg.DataRpc
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
        public decimal Mean
        {
            get;
            set;
        }

        [JsonProperty("standardDeviation", Required = Required.Always)]
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