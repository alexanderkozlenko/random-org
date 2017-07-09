using System;
using Newtonsoft.Json;

namespace Community.RandomOrg.Converters
{
    internal sealed class RandomTimeIntervalConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) =>
            objectType == typeof(long);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
            TimeSpan.FromMilliseconds((long)reader.Value);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            throw new NotSupportedException();

        public override bool CanWrite =>
            false;
    }
}