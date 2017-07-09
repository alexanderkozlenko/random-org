using System;
using Community.RandomOrg.Converters;
using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal sealed class RpcGetUsageResult : RpcMethodResult
    {
        [JsonProperty("status")]
        [JsonConverter(typeof(ApiKeyStatusConverter))]
        public ApiKeyStatus Status { get; set; }

        [JsonProperty("bitsLeft")]
        public long BitsLeft { get; set; }

        [JsonProperty("requestsLeft")]
        public long RequestsLeft { get; set; }

        [JsonProperty("creationTime")]
        public DateTime CreationTime { get; set; }

        [JsonProperty("totalBits")]
        public long TotalBits { get; set; }

        [JsonProperty("totalRequests")]
        public long TotalRequests { get; set; }
    }
}