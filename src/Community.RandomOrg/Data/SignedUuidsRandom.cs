using System;

namespace Community.RandomOrg.Data
{
    /// <summary>Encapsulates the signed random UUIDs and associated data.</summary>
    public sealed class SignedUuidsRandom : SignedRandom<Guid>
    {
        /// <summary>Initializes a new instance of the <see cref="SignedUuidsRandom" /> class.</summary>
        public SignedUuidsRandom()
        {
        }
    }
}