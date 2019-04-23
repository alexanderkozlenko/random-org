// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Anemonis.RandomOrg.DataRpc
{
    internal abstract class RpcRandomObject<T>
    {
        [JsonProperty("completionTime", Required = Required.Always)]
        public DateTime CompletionTime
        {
            get;
            set;
        }

        [JsonProperty("data", Required = Required.Always)]
        public IReadOnlyList<T> Data
        {
            get;
            set;
        }
    }
}
