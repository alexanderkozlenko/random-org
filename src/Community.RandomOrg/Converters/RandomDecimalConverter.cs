using System;
using Community.RandomOrg.Internal;
using Newtonsoft.Json;

namespace Community.RandomOrg.Converters
{
    internal sealed class RandomDecimalConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(decimal);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<decimal>(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(RandomOrgConvert.DecimalToObject((decimal)value));
        }
    }
}