using System;
using Community.RandomOrg.Converters;
using Newtonsoft.Json;

namespace Community.RandomOrg.Internal
{
    internal abstract class RpcRandomResultObject<TRandom, TValue> : RpcMethodResult
        where TRandom : RpcRandomObject<TValue>
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
        [JsonConverter(typeof(RandomTimeSpanConverter))]
        public TimeSpan AdvisoryDelay
        {
            get;
            set;
        }
    }
}