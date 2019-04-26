// © Alexander Kozlenko. Licensed under the MIT License.

using Newtonsoft.Json;

#pragma warning disable CA1812

namespace Anemonis.RandomOrg.DataRpc
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

#pragma warning restore CA1812
