// © Alexander Kozlenko. Licensed under the MIT License.

using Anemonis.RandomOrg.Data;

using Newtonsoft.Json;

#pragma warning disable CA1812

namespace Anemonis.RandomOrg.DataRpc
{
    internal sealed class RpcRandomUsageResult : RpcMethodResult
    {
        [JsonProperty("status", Required = Required.Always)]
        public ApiKeyStatus Status
        {
            get;
            set;
        }

        [JsonProperty("bitsLeft", Required = Required.Always)]
        public long BitsLeft
        {
            get;
            set;
        }

        [JsonProperty("requestsLeft", Required = Required.Always)]
        public long RequestsLeft
        {
            get;
            set;
        }
    }
}
