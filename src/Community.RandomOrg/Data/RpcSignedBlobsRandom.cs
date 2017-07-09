using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal sealed class RpcSignedBlobsRandom : RpcSignedRandom<string>
    {
        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }
    }
}