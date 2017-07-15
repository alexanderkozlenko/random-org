using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal abstract class RpcGenerateParams : RpcMethodParams
    {
        [JsonProperty("apiKey")]
        public string ApiKey { get; set; }

        [JsonProperty("n")]
        public long Count { get; set; }

        [JsonProperty("userData", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string UserData { get; set; }
    }
}