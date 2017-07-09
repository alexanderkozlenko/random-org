using System;
using Community.RandomOrg.Converters;
using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal sealed class RpcRandomResult<T> : RpcMethodResult, IAdvisoryDelayAware
    {
        [JsonProperty("random")]
        public RpcRandom<T> Random { get; set; }

        [JsonProperty("bitsLeft")]
        public long BitsLeft { get; set; }

        [JsonProperty("requestsLeft")]
        public long RequestsLeft { get; set; }

        [JsonProperty("bitsUsed")]
        public long BitsUsed { get; set; }

        [JsonProperty("advisoryDelay")]
        [JsonConverter(typeof(RandomTimeIntervalConverter))]
        public TimeSpan AdvisoryDelay { get; set; }
    }
}