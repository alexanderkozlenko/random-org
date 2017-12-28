using System.Buffers;
using Newtonsoft.Json;

namespace Community.RandomOrg
{
    internal sealed class JsonBufferPool : IArrayPool<char>
    {
        private readonly ArrayPool<char> _arrayPool = ArrayPool<char>.Create();

        public char[] Rent(int minimumLength)
        {
            return _arrayPool.Rent(minimumLength);
        }

        public void Return(char[] array)
        {
            _arrayPool.Return(array);
        }
    }
}