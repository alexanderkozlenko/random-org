using System;
using Newtonsoft.Json;

namespace Community.RandomOrg.Converters
{
    internal sealed class StringToUriConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) =>
            objectType == typeof(Uri);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            writer.WriteValue(((Uri)value)?.OriginalString);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
            reader.Value != null ? new Uri((string)reader.Value, UriKind.Absolute) : null;
    }
}