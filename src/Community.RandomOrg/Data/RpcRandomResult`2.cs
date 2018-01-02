using System;
using Community.RandomOrg.Converters;
using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal abstract class RpcRandomResult<TRandom, TValue> : RpcMethodResult
        where TRandom : RpcRandom<TValue>
    {
        [JsonProperty("random", Required = Required.Always)]
        public TRandom Random
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

        [JsonProperty("bitsUsed", Required = Required.Always)]
        public long BitsUsed
        {
            get;
            set;
        }

        [JsonProperty("advisoryDelay", Required = Required.Always)]
        [JsonConverter(typeof(RandomTimeIntervalConverter))]
        public TimeSpan AdvisoryDelay
        {
            get;
            set;
        }
    }
}