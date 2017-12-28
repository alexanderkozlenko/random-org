using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal sealed class RpcVerifyResult : RpcMethodResult
    {
        [JsonProperty("authenticity")]
        public bool Authenticity
        {
            get;
            set;
        }
    }
}