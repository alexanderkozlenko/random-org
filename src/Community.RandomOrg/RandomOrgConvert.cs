using System;

namespace Community.RandomOrg
{
    internal static class RandomOrgConvert
    {
        public static object ToObject(decimal value)
        {
            return value % 1 != 0 ? (object)value : Convert.ToInt64(value);
        }
    }
}