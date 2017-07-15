using System.ComponentModel;
using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal sealed class RpcGenerateStringsParams : RpcGenerateParams
    {
        [JsonProperty("length")]
        public long Length { get; set; }

        [JsonProperty("characters")]
        public string Characters { get; set; }

        [JsonProperty("replacement", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue(true)]
        public bool Replacement { get; set; }
    }
}