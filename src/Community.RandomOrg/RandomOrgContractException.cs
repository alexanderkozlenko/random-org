using System;

namespace Community.RandomOrg
{
    /// <summary>Represents an error that occur during RANDOM.ORG result handling.</summary>
    public sealed class RandomOrgContractException : Exception
    {
        internal RandomOrgContractException(string method, string message)
            : base(message)
        {
            Method = method;
        }

        internal RandomOrgContractException(string method, string message, Exception inner)
            : base(message, inner)
        {
            Method = method;
        }

        /// <summary>Gets the method name.</summary>
        public string Method
        {
            get;
        }
    }
}