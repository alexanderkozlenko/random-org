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
                        throw new JsonException(Strings.GetString("protocol.random.api_key_status.invalid_value"));
                    }
            }
        }

        public override void WriteJson(JsonWriter writer, ApiKeyStatus value, JsonSerializer serializer)
        {
        }
    }
}