using System;
using Newtonsoft.Json;

namespace Community.RandomOrg.Converters
{
    internal sealed class RandomDecimalConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) =>
            objectType == typeof(decimal);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
            serializer.Deserialize<decimal>(reader);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            writer.WriteValue((decimal)value % 1 != 0 ? value : Convert.ToInt64(value));
    }
}