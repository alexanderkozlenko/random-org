using Community.RandomOrg.Converters;
using Community.RandomOrg.Data;
using Newtonsoft.Json;

namespace Community.RandomOrg.Internal
{
    internal sealed class RpcRandomUsageResult : RpcMethodResult
    {
        [JsonProperty("status", Required = Required.Always)]
        [JsonConverter(typeof(ApiKeyStatusConverter))]
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