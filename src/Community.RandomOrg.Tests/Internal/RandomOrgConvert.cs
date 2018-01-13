using System;
using System.Diagnostics;
using System.Globalization;
using Community.RandomOrg.Data;

namespace Community.RandomOrg.Tests.Internal
{
    /// <summary>Represents a converter for RANDOM.ORG JSON values.</summary>
    [DebuggerStepThrough]
    internal static class RandomOrgConvert
    {
        /// <summary>Converts a string value to a <see cref="DateTime" /> value.</summary>
        /// <param name="value">A string with a serialized date and time value.</param>
        /// <returns>A date and time value.</returns>
        public static DateTime ToDateTime(string value)
        {
            return DateTime.ParseExact(value, "yyyy'-'MM'-'dd' 'HH':'mm':'ss.FFFFFFFK", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
        }

        /// <summary>Converts a string value to a <see cref="ApiKeyStatus" /> value.</summary>
        /// <param name="value">A string with a serialized API key status value.</param>
        /// <returns>A <see cref="ApiKeyStatus" /> value.</returns>
        /// <exception cref="ArgumentException"><paramref name="value" /> contains invalid value for API key status.</exception>
        public static ApiKeyStatus ToApiKeyStatus(string value)
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