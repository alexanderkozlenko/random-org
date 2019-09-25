// © Alexander Kozlenko. Licensed under the MIT License.

using Newtonsoft.Json;

#pragma warning disable CA1812

namespace Anemonis.RandomOrg.DataRpc
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
