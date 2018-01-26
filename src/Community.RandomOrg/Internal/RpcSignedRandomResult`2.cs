﻿using Newtonsoft.Json;

namespace Community.RandomOrg.Internal
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