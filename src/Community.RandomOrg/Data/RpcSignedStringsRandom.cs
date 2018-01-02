using System.ComponentModel;
using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal sealed class RpcSignedStringsRandom : RpcSignedRandom<string>
    {
        [JsonProperty("n", Required = Required.Always)]
        public long Count
        {
            get;
            set;
        }

        [JsonProperty("length", Required = Required.Always)]
        public long Length
        {
            get;
            set;
        }

        [JsonProperty("characters", Required = Required.Always)]
        public string Characters
        {
            get;
            set;
        }

        [JsonProperty("replacement", Required = Required.Always)]
        [DefaultValue(true)]
        public bool Replacement
        {
            get;
            set;
        }
    }
}