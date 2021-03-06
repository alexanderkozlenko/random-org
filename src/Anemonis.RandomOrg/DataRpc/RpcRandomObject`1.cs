﻿// © Alexander Kozlenko. Licensed under the MIT License.

using System.Collections.Generic;

using Newtonsoft.Json;

#pragma warning disable CA1812

namespace Anemonis.RandomOrg.DataRpc
{
    internal abstract class RpcRandomObject<T> : RpcRandomObject
    {
        [JsonProperty("data", Required = Required.Always)]
        public IReadOnlyList<T> Data
        {
            get;
            set;
        }
    }
}
