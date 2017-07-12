using System;

namespace Community.RandomOrg
{
    /// <summary>Represents error that occur during RANDOM.ORG method invocation.</summary>
    public sealed class RandomOrgException : Exception
    {
        internal RandomOrgException(long code, string message)
            : base(message)
        {
            Code = code;
        }

        /// <summary>Gets a number that indicates the error type that occurred.</summary>
        public long Code { get; }
    }
}