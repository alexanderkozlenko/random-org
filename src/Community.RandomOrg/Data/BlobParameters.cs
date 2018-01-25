namespace Community.RandomOrg.Data
{
    /// <summary>Encapsulates random BLOBs generation parameters.</summary>
    public sealed class BlobParameters : RandomParameters
    {
        /// <summary>Initializes a new instance of the <see cref="BlobParameters" /> class.</summary>
        public BlobParameters()
        {
        }

        /// <summary>Gets or sets the size of each blob, measured in bytes.</summary>
        public int Size
        {
            get;
            set;
        }
    }
}