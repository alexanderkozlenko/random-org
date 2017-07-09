using System;
using Newtonsoft.Json;

namespace Community.RandomOrg.Converters
{
    internal sealed class Base64ToByteArrayConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) =>
            objectType == typeof(string);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
            Convert.FromBase64String((string)reader.Value ?? string.Empty);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            writer.WriteValue(Convert.ToBase64String((byte[])value ?? new byte[] { }));
    }
}