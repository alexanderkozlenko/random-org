// © Alexander Kozlenko. Licensed under the MIT License.

using Newtonsoft.Json;

namespace Anemonis.RandomOrg.DataRpc
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
        public long AdvisoryDelay
        {
            get;
            set;
        }
    }
}