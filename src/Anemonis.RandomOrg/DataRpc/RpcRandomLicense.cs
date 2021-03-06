﻿// © Alexander Kozlenko. Licensed under the MIT License.

using Newtonsoft.Json;

#pragma warning disable CA1812

namespace Anemonis.RandomOrg.DataRpc
{
    internal sealed class RpcRandomLicense
    {
        [JsonProperty("type", Required = Required.Always)]
        public string Type
        {
            get;
            set;
        }

        [JsonProperty("text")]
        public string Text
        {
            get;
            set;
        }

        [JsonProperty("infoUrl")]
        public string InfoUrl
        {
            get;
            set;
        }
    }
}
