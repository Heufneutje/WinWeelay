using System;
using System.Security.Cryptography;
using System.Text;

namespace WinWeelay.Utils
{
    /// <summary>
    /// Utility class for encrypting and decrypting data.
    /// </summary>
    public static class Cipher
    {
        /// <summary>
        /// Encrypt a given string.
        /// </summary>
        /// <param name="plainText">The text to encrypt.</param>
        /// <returns>The encrypted text.</returns>
        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return null;

            byte[] passBytes = ProtectedData.Protect(Encoding.UTF8.GetBytes(plainText), null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(passBytes);
        }

        /// <summary>
        /// Decrypt a given string.
        /// </summary>
        /// <param name="cipherText">The given encrypted string.</param>
        /// <returns>The decrypted plain text string.</returns>
        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return null;

            try
            {
                byte[] passBytes = ProtectedData.Unprotect(Convert.FromBase64String(cipherText), null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(passBytes);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
