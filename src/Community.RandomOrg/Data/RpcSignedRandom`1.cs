using Community.RandomOrg.Converters;
using Newtonsoft.Json;

namespace Community.RandomOrg.Data
{
    internal abstract class RpcSignedRandom<T> : RpcRandom<T>
    {
        protected RpcSignedRandom()
        {
            License = new RpcLicense();
        }

        [JsonProperty("n")]
        public long Count
        {
            get;
            set;
        }

        [JsonProperty("method")]
        public string Method
        {
            get;
            set;
        }

        [JsonProperty("hashedApiKey")]
        [JsonConverter(typeof(Base64ToByteArrayConverter))]
        public byte[] ApiKeyHash
        {
            get;
            set;
        }

        [JsonProperty("serialNumber")]
        public long SerialNumber
        {
            get;
            set;
        }

        [JsonProperty("userData")]
        public string UserData
        {
            get;
            set;
        }

        [JsonProperty("license")]
        public RpcLicense License
        {
            get;
        }
    }
}