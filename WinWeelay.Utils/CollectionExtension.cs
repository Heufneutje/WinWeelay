namespace WinWeelay.Utils
{
    /// <summary>
    /// Utility class for array operations.
    /// </summary>
    public static class CollectionExtension
    {
        /// <summary>
        /// Copy a given section of an array.
        /// </summary>
        /// <typeparam name="T">Type of the given array.</typeparam>
        /// <param name="src">The given array.</param>
        /// <param name="start">The start index of the section.</param>
        /// <param name="end">The end index of the section.</param>
        /// <returns>A copy of the array section with the given indices.</returns>
        public static T[] CopyOfRange<T>(this T[] src, int start, int end)
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
