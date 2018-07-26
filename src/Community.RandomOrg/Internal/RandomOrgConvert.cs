// © Alexander Kozlenko. Licensed under the MIT License.

namespace Community.RandomOrg.Internal
{
    internal static class RandomOrgConvert
    {
        public static object DecimalToObject(in decimal value)
        {
            // The service uses numbers without specifying an empty fraction part

            return value % 1 != 0 ? (object)value : (long)value;
        }
    }
}