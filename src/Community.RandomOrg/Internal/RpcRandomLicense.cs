using System;
using Community.RandomOrg.Converters;
using Newtonsoft.Json;

namespace Community.RandomOrg.Internal
{
    internal sealed class RpcRandomLicense
    {
        [JsonProperty("type")]
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
        [JsonConverter(typeof(StringToUriConverter))]
        public Uri InfoUrl
        {
            get;
            set;
        }
    }
}