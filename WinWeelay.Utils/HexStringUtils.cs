using System;
using System.Linq;

namespace WinWeelay.Utils
{
    /// <summary>
    /// Helper class for converting between a string of hex values and a byte array.
    /// </summary>
    public static class HexStringUtils
    {
        /// <summary>
        /// Get a byte array from a given string of hex values.
        /// </summary>
        /// <param name="hexString">The hex values.</param>
        /// <returns>A byte array.</returns>
        public static byte[] ConvertHexStringToBytes(string hexString)
        {
            return Enumerable.Range(0, hexString.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                             .ToArray();
        }

        /// <summary>
        /// Get a hex value string from a given byte array.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>A string that contains the hex values of the bytes.</returns>
        public static string ConvertBytesToHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }
    }
}
