using System;
using System.Diagnostics;
using System.Globalization;
using Anemonis.RandomOrg.Data;

namespace Anemonis.RandomOrg.UnitTests.Internal
{
    [DebuggerStepThrough]
    internal static class RandomOrgConverter
    {
        public static DateTime StringToDateTime(string value)
        {
            return DateTime.ParseExact(value, "yyyy'-'MM'-'dd' 'HH':'mm':'ss.FFFFFFFK", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
        }

        public static ApiKeyStatus StringToApiKeyStatus(string value)
        {
            switch (value)
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
                        throw new ArgumentException($"The specified value is not supported: \"{value}\"", nameof(value));
                    }
            }
        }
    }
}