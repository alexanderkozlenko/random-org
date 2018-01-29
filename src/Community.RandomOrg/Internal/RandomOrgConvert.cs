namespace Community.RandomOrg.Internal
{
    internal static class RandomOrgConvert
    {
        public static object DecimalToObject(decimal value)
        {
            return value % 1 != 0 ? (object)value : (long)value;
        }
    }
}