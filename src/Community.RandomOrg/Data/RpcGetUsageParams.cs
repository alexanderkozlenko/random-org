using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal sealed class RpcGetUsageParams : RpcMethodParams
    {
        [JsonProperty("apiKey")]
        public string ApiKey { get; set; }
    }
}