// © Alexander Kozlenko. Licensed under the MIT License.

using Newtonsoft.Json;

#pragma warning disable CA1812

namespace Anemonis.RandomOrg.DataRpc
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
        public bool Replacement
        {
            get;
            set;
        }
    }
}
