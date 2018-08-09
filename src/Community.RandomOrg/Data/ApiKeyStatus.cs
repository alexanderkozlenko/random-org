// © Alexander Kozlenko. Licensed under the MIT License.

namespace Community.RandomOrg.Data
{
    /// <summary>The status of an API key.</summary>
    public enum ApiKeyStatus
    {
        /// <summary>The key is in stopped state.</summary>
        Stopped,

        /// <summary>The key is in paused state.</summary>
        Paused,

        /// <summary>The key is active.</summary>
        Running
    }
}