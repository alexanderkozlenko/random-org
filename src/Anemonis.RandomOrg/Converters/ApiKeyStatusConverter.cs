// © Alexander Kozlenko. Licensed under the MIT License.

using System;

using Anemonis.RandomOrg.Data;
using Anemonis.RandomOrg.Resources;

using Newtonsoft.Json;

namespace Anemonis.RandomOrg.Converters
{
    internal sealed class ApiKeyStatusConverter : JsonConverter<ApiKeyStatus>
    {
        public override ApiKeyStatus ReadJson(JsonReader reader, Type objectType, ApiKeyStatus existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                    {
                        switch ((string)reader.Value)
                        {
                            case "stopped":
                                {
                                    return ApiKeyStatus.Stopped;
                                }
                            case "running":
                                {
                                    return ApiKeyStatus.Running;
                                }
                        }
                    }
                    break;
            }

            throw new JsonSerializationException(string.Format(Strings.GetString("json.invalid_json_string"), objectType));
        }

        public override void WriteJson(JsonWriter writer, ApiKeyStatus value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }
    }
}
