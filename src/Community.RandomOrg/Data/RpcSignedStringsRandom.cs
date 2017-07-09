using System.ComponentModel;
using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal sealed class RpcSignedStringsRandom : RpcSignedRandom<string>
    {
        [JsonProperty("length")]
        public long Length { get; set; }

        [JsonProperty("characters")]
        public string Characters { get; set; }

        [JsonProperty("replacement")]
        [DefaultValue(true)]
        public bool Replacement { get; set; }
    }
}