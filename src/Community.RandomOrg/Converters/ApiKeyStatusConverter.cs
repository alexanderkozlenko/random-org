using System;
using Community.RandomOrg.Data;
using Community.RandomOrg.Resources;
using Newtonsoft.Json;

namespace Community.RandomOrg.Converters
{
    internal sealed class ApiKeyStatusConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
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

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }
    }
}