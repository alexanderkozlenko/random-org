// © Alexander Kozlenko. Licensed under the MIT License.

using Newtonsoft.Json;

namespace Community.RandomOrg.Internal
{
    internal sealed class RpcVerifyResult : RpcMethodResult
    {
        [JsonProperty("authenticity", Required = Required.Always)]
        public bool Authenticity
        {
            get;
            set;
        }
    }
}