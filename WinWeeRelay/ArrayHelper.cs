namespace WinWeeRelay
{
    public static class ArrayHelper
    {
        public static T[] CopyOfRange<T>(T[] src, int start, int end)
        {
            int len = end - start;
            T[] dest = new T[len];
            for (int i = 0; i < len; i++)
            {
                dest[i] = src[start + i];
            }
            return dest;
        }
    }
}
