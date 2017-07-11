using Community.RandomOrg.Converters;
using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal sealed class RpcSignedRandomResult<TRandom, TValue> : RpcRandomResult<TRandom, TValue>
        where TRandom : RpcSignedRandom<TValue>
    {
        [JsonProperty("signature")]
        [JsonConverter(typeof(Base64ToByteArrayConverter))]
        public byte[] Signature { get; set; }
    }
}