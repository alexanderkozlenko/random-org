// © Alexander Kozlenko. Licensed under the MIT License.

using Newtonsoft.Json;

namespace Community.RandomOrg.DataRpc
{
    internal sealed class RpcSignedRandomResult<TRandom, TValue> : RpcRandomResultObject<TRandom, TValue>
        where TRandom : RpcSignedRandom<TValue>
    {
        [JsonProperty("signature", Required = Required.Always)]
        public byte[] Signature
        {
            get;
            set;
        }
    }
}