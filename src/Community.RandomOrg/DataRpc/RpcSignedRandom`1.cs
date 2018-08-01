// © Alexander Kozlenko. Licensed under the MIT License.

using Newtonsoft.Json;

namespace Community.RandomOrg.DataRpc
{
    internal abstract class RpcSignedRandom<T> : RpcRandomObject<T>
    {
        protected RpcSignedRandom()
        {
            License = new RpcRandomLicense();
        }

        [JsonProperty("method", Required = Required.Always)]
        public string Method
        {
            get;
            set;
        }

        [JsonProperty("hashedApiKey", Required = Required.Always)]
        public byte[] ApiKeyHash
        {
            get;
            set;
        }

        [JsonProperty("serialNumber", Required = Required.Always)]
        public long SerialNumber
        {
            get;
            set;
        }

        [JsonProperty("userData", Required = Required.AllowNull)]
        public string UserData
        {
            get;
            set;
        }

        [JsonProperty("license", Required = Required.Always)]
        public RpcRandomLicense License
        {
            get;
        }
    }
}