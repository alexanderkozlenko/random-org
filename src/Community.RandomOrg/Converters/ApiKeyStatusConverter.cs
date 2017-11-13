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
            return objectType == typeof(string);
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
                        throw new NotSupportedException(Strings.GetString("Service.ApiKeyStatusIsInvalid"));
                    }
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }
    }
}