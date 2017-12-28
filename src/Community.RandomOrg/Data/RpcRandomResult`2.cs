using System;
using Community.RandomOrg.Converters;
using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal abstract class RpcRandomResult<TRandom, TValue> : RpcMethodResult
        where TRandom : RpcRandom<TValue>
    {
        [JsonProperty("random")]
        public TRandom Random
        {
            get;
            set;
        }

        [JsonProperty("bitsLeft")]
        public long BitsLeft
        {
            get;
            set;
        }

        [JsonProperty("requestsLeft")]
        public long RequestsLeft
        {
            get;
            set;
        }

        [JsonProperty("bitsUsed")]
        public long BitsUsed
        {
            get;
            set;
        }

        [JsonProperty("advisoryDelay")]
        [JsonConverter(typeof(RandomTimeIntervalConverter))]
        public TimeSpan AdvisoryDelay
        {
            get;
            set;
        }
    }
}