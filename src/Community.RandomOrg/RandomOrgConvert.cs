// © Alexander Kozlenko. Licensed under the MIT License.

namespace Community.RandomOrg
{
    internal static class RandomOrgConvert
    {
        public static object DecimalToObject(in decimal value)
        {
            return value % 1 != 0 ? (object)value : (object)(long)value;
        }
    }
}