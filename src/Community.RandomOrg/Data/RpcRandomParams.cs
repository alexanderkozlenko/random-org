using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal abstract class RpcRandomParams : RpcMethodParams
    {
        [JsonProperty("apiKey")]
        public string ApiKey { get; set; }

        [JsonProperty("n")]
        public long Count { get; set; }
    }
}