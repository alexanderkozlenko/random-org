﻿using Newtonsoft.Json.Converters;

namespace Community.RandomOrg.Converters
{
    internal sealed class RandomDateTimeConverter : IsoDateTimeConverter
    {
        public RandomDateTimeConverter() =>
            DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss.FFFFFFFK";
    }
}