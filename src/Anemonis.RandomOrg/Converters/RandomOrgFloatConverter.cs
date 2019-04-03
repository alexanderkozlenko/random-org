// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using Anemonis.RandomOrg.Resources;
using Newtonsoft.Json;

namespace Anemonis.RandomOrg.Converters
{
    internal sealed class RandomOrgFloatConverter : JsonConverter<decimal>
    {
        public override decimal ReadJson(JsonReader reader, Type objectType, decimal existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Integer:
                    {
                        return new decimal((long)reader.Value);
                    }
                case JsonToken.Float:
                    {
                        return new decimal((double)reader.Value);
                    }
            }

            throw new JsonSerializationException(string.Format(Strings.GetString("json.invalid_json_string"), objectType));
        }

        public override void WriteJson(JsonWriter writer, decimal value, JsonSerializer serializer)
        {
            if (value % 1 == 0)
            {
                writer.WriteValue(decimal.ToInt64(value));
            }
            else
            {
                writer.WriteValue(value);
            }
        }
    }
}