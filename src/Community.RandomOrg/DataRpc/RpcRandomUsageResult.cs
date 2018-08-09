﻿// © Alexander Kozlenko. Licensed under the MIT License.

using Community.RandomOrg.Data;
using Newtonsoft.Json;

namespace Community.RandomOrg.DataRpc
{
    internal sealed class RpcRandomUsageResult : RpcMethodResult
    {
        [JsonProperty("status", Required = Required.Always)]
        public ApiKeyStatus Status
        {
            get;
            set;
        }

        [JsonProperty("bitsLeft", Required = Required.Always)]
        public long BitsLeft
        {
            get;
            set;
        }

        [JsonProperty("requestsLeft", Required = Required.Always)]
        public long RequestsLeft
        {
            get;
            set;
        }
    }
}