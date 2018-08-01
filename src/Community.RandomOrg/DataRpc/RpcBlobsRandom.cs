// © Alexander Kozlenko. Licensed under the MIT License.

using Newtonsoft.Json;

namespace Community.RandomOrg.DataRpc
{
    internal sealed class RpcBlobsRandom : RpcSignedRandom<byte[]>
    {
        [JsonProperty("n", Required = Required.Always)]
        public int Count
        {
            get;
            set;
        }

        [JsonProperty("size", Required = Required.Always)]
        public int Size
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