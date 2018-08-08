// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using Community.RandomOrg.Data;
using Community.RandomOrg.Resources;
using Newtonsoft.Json;

namespace Community.RandomOrg.Converters
{
    internal sealed class ApiKeyStatusConverter : JsonConverter<ApiKeyStatus>
    {
        public override ApiKeyStatus ReadJson(JsonReader reader, Type objectType, ApiKeyStatus existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            switch ((string)reader.Value)
            {
                case "stopped":
                    {
                        return ApiKeyStatus.Stopped;
                    }
                case "paused":
                    {
                        return ApiKeyStatus.Paused;
                    }
                case "running":
                    {
                        return ApiKeyStatus.Running;
                    }
                default:
                    {
                        throw new JsonSerializationException(string.Format(Strings.GetString("json.invalid_json_string"), objectType));
                    }
            }
        }

        public override void WriteJson(JsonWriter writer, ApiKeyStatus value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }
    }
}