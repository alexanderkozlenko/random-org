using System;
using Newtonsoft.Json;

namespace Community.RandomOrg.Converters
{
    internal sealed class Base64ToByteArrayConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value != null)
            {
                return Convert.FromBase64String((string)reader.Value);
            }
            else
            {
                return null;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null)
            {
                writer.WriteValue(Convert.ToBase64String((byte[])value));
            }
            else
            {
                writer.WriteNull();
            }
        }
    }
}