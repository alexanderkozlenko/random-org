using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal sealed class RpcGenerateBlobsParams : RpcRandomParams
    {
        [JsonProperty("size")]
        public long Size { get; set; }
    }
}