using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal sealed class RpcGetResultParams : RpcMethodParams
    {
        [JsonProperty("apiKey")]
        public string ApiKey { get; set; }

        [JsonProperty("serialNumber")]
        public long SerialNumber { get; set; }
    }
}