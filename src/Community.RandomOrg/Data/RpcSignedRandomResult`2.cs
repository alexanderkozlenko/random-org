using System;
using Community.RandomOrg.Converters;
using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal sealed class RpcSignedRandomResult<TRandom, TValue> : RpcMethodResult, IAdvisoryDelayAware
        where TRandom : RpcSignedRandom<TValue>
    {
        [JsonProperty("random")]
        public TRandom Random { get; set; }

        [JsonProperty("bitsLeft")]
        public long BitsLeft { get; set; }

        [JsonProperty("requestsLeft")]
        public long RequestsLeft { get; set; }

        [JsonProperty("bitsUsed")]
        public long BitsUsed { get; set; }

        [JsonProperty("advisoryDelay")]
        [JsonConverter(typeof(RandomTimeIntervalConverter))]
        public TimeSpan AdvisoryDelay { get; set; }

        [JsonProperty("signature")]
        [JsonConverter(typeof(Base64ToByteArrayConverter))]
        public byte[] Signature { get; set; }
    }
}