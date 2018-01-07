using System;
using Community.RandomOrg.Converters;
using Community.RandomOrg.Data;
using Newtonsoft.Json;

namespace Community.RandomOrg.Internal
{
    internal sealed class RpcGetUsageResult : RpcMethodResult
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

        [JsonProperty("creationTime", Required = Required.Always)]
        public DateTime CreationTime
        {
            get;
            set;
        }

        [JsonProperty("totalBits", Required = Required.Always)]
        public long TotalBits
        {
            get;
            set;
        }

        [JsonProperty("totalRequests", Required = Required.Always)]
        public long TotalRequests
        {
            get;
            set;
        }
    }
}