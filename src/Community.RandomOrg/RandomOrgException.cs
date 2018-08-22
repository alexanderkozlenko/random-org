// © Alexander Kozlenko. Licensed under the MIT License.

using System;

namespace Community.RandomOrg
{
    /// <summary>Represents an error that occurs during invocation of a RANDOM.ORG service method.</summary>
    public sealed class RandomOrgException : Exception
    {
        internal RandomOrgException(string method, long code, string message)
            : base(message)
        {
            Method = method;
            Code = code;
        }

        /// <summary>Gets the method name.</summary>
        public string Method
        {
            get;
        }

        /// <summary>Gets a number that indicates the error type that occurred.</summary>
        public long Code
        {
            get;
        }
    }
}