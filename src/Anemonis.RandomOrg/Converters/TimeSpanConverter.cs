// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Globalization;

using Anemonis.RandomOrg.Resources;

using Newtonsoft.Json;

namespace Anemonis.RandomOrg.Converters
{
    internal sealed class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan ReadJson(JsonReader reader, Type objectType, TimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Integer:
                    {
                        return TimeSpan.FromMilliseconds((long)reader.Value);
                    }
            }

            throw new JsonSerializationException(string.Format(CultureInfo.CurrentCulture, Strings.GetString("json.invalid_json_string"), objectType));
        }

        public override void WriteJson(JsonWriter writer, TimeSpan value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }
    }
}
