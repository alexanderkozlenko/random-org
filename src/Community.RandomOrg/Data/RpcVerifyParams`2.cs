using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal sealed class RpcVerifyParams<TRandom, TValue> : RpcVerifyParams
        where TRandom : RpcSignedRandom<TValue>
    {
        [JsonProperty("random")]
        public TRandom Random { get; set; }
    }
}