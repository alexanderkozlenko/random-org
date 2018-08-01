// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using Newtonsoft.Json;

namespace Community.RandomOrg.DataRpc
{
    internal sealed class RpcUuidsRandom : RpcSignedRandom<Guid>
    {
        [JsonProperty("n", Required = Required.Always)]
        public int Count
        {
            get;
            set;
        }
    }
}