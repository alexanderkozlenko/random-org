﻿// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using Newtonsoft.Json;

namespace Community.RandomOrg.Converters
{
    internal sealed class RandomDecimalConverter : JsonConverter<decimal>
    {
        public override decimal ReadJson(JsonReader reader, Type objectType, decimal existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<decimal>(reader);
        }

        public override void WriteJson(JsonWriter writer, decimal value, JsonSerializer serializer)
        {
            writer.WriteValue(RandomOrgConvert.DecimalToObject(value));
        }
    }
}