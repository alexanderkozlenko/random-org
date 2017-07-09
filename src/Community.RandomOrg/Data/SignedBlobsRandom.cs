namespace Community.RandomOrg.Data
{
    /// <summary>Encapsulates the signed random BLOBs and associated data.</summary>
    public sealed class SignedBlobsRandom : SignedRandom<byte[]>
    {
        /// <summary>Gets or sets the size of each blob, measured in bits.</summary>
        public int Size { get; set; }
    }
}