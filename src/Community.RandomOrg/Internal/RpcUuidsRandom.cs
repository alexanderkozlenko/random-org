using System;
using Newtonsoft.Json;

namespace Community.RandomOrg.Internal
{
    internal sealed class RpcUuidsRandom : RpcSignedRandom<Guid>
    {
        [JsonProperty("n", Required = Required.Always)]
        public int Count
        {
            get;
            set;
        }
    }
}