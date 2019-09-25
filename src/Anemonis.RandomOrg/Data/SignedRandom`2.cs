// © Alexander Kozlenko. Licensed under the MIT License.

#pragma warning disable CA1819

namespace Anemonis.RandomOrg.Data
{
    /// <summary>Encapsulates signed random objects and associated data.</summary>
    /// <typeparam name="TValue">The type of random object.</typeparam>
    /// <typeparam name="TParameters">The type of random parameters.</typeparam>
    public sealed class SignedRandom<TValue, TParameters> : RandomObject<TValue>
        where TParameters : RandomParameters, new()
    {
        /// <summary>Initializes a new instance of the <see cref="SignedRandom{TValue, TParameters}" /> class.</summary>
        public SignedRandom()
        {
            Parameters = new TParameters();
            License = new RandomLicense();
        }

        /// <summary>Gets the random parameters.</summary>
        public TParameters Parameters
        {
            get;
        }

        /// <summary>Gets or sets the SHA-512 hash of the API key.</summary>
        public byte[] ApiKeyHash
        {
            get;
            set;
        }

        /// <summary>Gets or sets an integer containing the serial number associated with the random information.</summary>
        public long SerialNumber
        {
            get;
            set;
        }

        /// <summary>Gets or sets an optional string that is included into signed data from generation parameters.</summary>
        public string UserData
        {
            get;
            set;
        }

        /// <summary>Gets an object describing the license terms under which the random values can be used.</summary>
        public RandomLicense License
        {
            get;
        }
    }
}
