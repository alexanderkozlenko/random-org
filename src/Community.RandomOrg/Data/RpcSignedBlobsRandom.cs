using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal sealed class RpcSignedBlobsRandom : RpcSignedRandom<string>
    {
        [JsonProperty("n", Required = Required.Always)]
        public long Count
        {
            get;
            set;
        }

        [JsonProperty("size", Required = Required.Always)]
        public long Size
        {
            get;
            set;
        }

        [JsonProperty("format", Required = Required.Always)]
        public string Format
        {
            get;
            set;
        }
    }
}