﻿using System.ComponentModel;
using Newtonsoft.Json;

namespace Community.RandomOrg.Internal
{
    internal sealed class RpcStringsRandom : RpcSignedRandom<string>
    {
        [JsonProperty("n", Required = Required.Always)]
        public int Count
        {
            get;
            set;
        }

        [JsonProperty("length", Required = Required.Always)]
        public int Length
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