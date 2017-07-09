using System;
using System.Collections.Generic;
using Community.RandomOrg.Converters;
using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal abstract class RpcSignedRandom<T>
    {
        [JsonProperty("n")]
        public long Count { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("hashedApiKey")]
        [JsonConverter(typeof(Base64ToByteArrayConverter))]
        public byte[] ApiKeyHash { get; set; }

        [JsonProperty("completionTime")]
        [JsonConverter(typeof(RandomDateTimeConverter))]
        public DateTime CompletionTime { get; set; }

        [JsonProperty("serialNumber")]
        public long SerialNumber { get; set; }

        [JsonProperty("data")]
        public IReadOnlyList<T> Data { get; set; }
    }
}