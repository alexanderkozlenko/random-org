using System;
using System.Collections.Generic;
using Community.RandomOrg.Converters;
using Newtonsoft.Json;

namespace Community.RandomOrg.Internal
{
    internal abstract class RpcRandomObject<T>
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