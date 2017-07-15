using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal sealed class RpcGenerateBlobsParams : RpcGenerateParams
    {
        [JsonProperty("size")]
        public long Size { get; set; }
    }
}