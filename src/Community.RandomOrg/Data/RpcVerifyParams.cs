using Community.RandomOrg.Converters;
using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal abstract class RpcVerifyParams : RpcMethodParams
    {
        [JsonProperty("signature")]
        [JsonConverter(typeof(Base64ToByteArrayConverter))]
        public byte[] Signature { get; set; }
    }
}