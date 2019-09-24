// © Alexander Kozlenko. Licensed under the MIT License.

using System;

using Newtonsoft.Json;

#pragma warning disable CA1812

namespace Anemonis.RandomOrg.DataRpc
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
