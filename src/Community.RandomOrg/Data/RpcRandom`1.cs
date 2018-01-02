using System;
using System.Collections.Generic;
using Community.RandomOrg.Converters;
using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal abstract class RpcRandom<T>
    {
        [JsonProperty("completionTime", Required = Required.Always)]
        [JsonConverter(typeof(RandomDateTimeConverter))]
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