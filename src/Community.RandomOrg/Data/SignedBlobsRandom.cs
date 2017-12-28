namespace Community.RandomOrg.Data
{
    /// <summary>Encapsulates the signed random BLOBs and associated data.</summary>
    public sealed class SignedBlobsRandom : SignedRandom<byte[]>
    {
        /// <summary>Initializes a new instance of the <see cref="SignedBlobsRandom" /> class.</summary>
        public SignedBlobsRandom()
        {
        }

        /// <summary>Gets or sets the size of each blob, measured in bits.</summary>
        public int Size
        {
            get;
            set;
        }
    }
}