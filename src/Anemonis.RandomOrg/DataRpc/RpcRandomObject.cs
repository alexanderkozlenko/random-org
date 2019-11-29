// © Alexander Kozlenko. Licensed under the MIT License.

using System;

using Newtonsoft.Json;

#pragma warning disable CA1812

namespace Anemonis.RandomOrg.DataRpc
{
    internal abstract class RpcRandomObject
    {
        [JsonProperty("completionTime", Required = Required.Always)]
        public DateTime CompletionTime
        {
            get;
            set;
        }
    }
}
