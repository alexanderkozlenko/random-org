using System;
using System.Reflection;
using System.Resources;
using Community.RandomOrg.Data;
using Newtonsoft.Json;

namespace Community.RandomOrg.Converters
{
    internal sealed class ApiKeyStatusConverter : JsonConverter
    {
        private static readonly ResourceManager _resourceManager = CreateResourceManager();

        private static ResourceManager CreateResourceManager()
        {
            var assembly = typeof(RandomOrgClient).GetTypeInfo().Assembly;

            return new ResourceManager($"{assembly.GetName().Name}.Resources.Strings", assembly);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch ((string)reader.Value)
            {
                case "stopped":
                    return ApiKeyStatus.Stopped;
                case "paused":
                    return ApiKeyStatus.Paused;
                case "running":
                    return ApiKeyStatus.Running;
                default:
                    throw new NotSupportedException(_resourceManager.GetString("Service.ApiKeyStatusIsInvalid"));
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        public override bool CanWrite
        {
            get => false;
        }
    }
}