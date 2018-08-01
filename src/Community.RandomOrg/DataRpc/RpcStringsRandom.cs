// © Alexander Kozlenko. Licensed under the MIT License.

using Newtonsoft.Json;

namespace Community.RandomOrg.DataRpc
{
    internal sealed class RpcStringsRandom : RpcSignedRandom<string>
    {
        [JsonProperty("n", Required = Required.Always)]
        public int Count
        {
            get;
            set;
        }

        [JsonProperty("length", Required = Required.Always)]
        public int Length
        {
            get;
            set;
        }

        [JsonProperty("characters", Required = Required.Always)]
        public string Characters
        {
            get;
            set;
        }

        [JsonProperty("replacement", Required = Required.Always)]
        public bool Replacement
        {
            get;
            set;
        }
    }
}