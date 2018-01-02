using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal sealed class RpcVerifyResult : RpcMethodResult
    {
        [JsonProperty("authenticity", Required = Required.Always)]
        public bool Authenticity
        {
            get;
            set;
        }
    }
}