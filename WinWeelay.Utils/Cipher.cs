using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace WinWeelay.Utils
{
    public static class Cipher
    {
        private const int KeySize = 256;

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return null;

            return Encrypt(plainText, GetUniqueID());
        }

        public static ICryptoTransform GetCryptoTransform(string passPhrase, byte[] saltStringBytes, byte[] ivStringBytes, bool encrypt)
        {
            const int derivationIterations = 1000;

            using (Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, derivationIterations))
            {
                byte[] keyBytes = password.GetBytes(KeySize / 8);
                using (RijndaelManaged symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;

                    if (encrypt)
                        return symmetricKey.CreateEncryptor(keyBytes, ivStringBytes);
                    else
                        return symmetricKey.CreateDecryptor(keyBytes, ivStringBytes);
                }
            }
        }

        public static string Encrypt(string plainText, string passPhrase)
        {
            byte[] saltStringBytes = Generate256BitsOfRandomEntropy();
            byte[] ivStringBytes = Generate256BitsOfRandomEntropy();
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (ICryptoTransform encryptor = GetCryptoTransform(passPhrase, saltStringBytes, ivStringBytes, true))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        cryptoStream.FlushFinalBlock();
                        byte[] cipherTextBytes = saltStringBytes;
                        cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                        cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                        memoryStream.Close();
                        cryptoStream.Close();
                        return Convert.ToBase64String(cipherTextBytes);
                    }
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return null;

            return Decrypt(cipherText, GetUniqueID());
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {
            byte[] cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            byte[] saltStringBytes = cipherTextBytesWithSaltAndIv.Take(KeySize / 8).ToArray();
            byte[] ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(KeySize / 8).Take(KeySize / 8).ToArray();
            byte[] cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((KeySize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((KeySize / 8) * 2)).ToArray();

            using (ICryptoTransform decryptor = GetCryptoTransform(passPhrase, saltStringBytes, ivStringBytes, false))
            {
                using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                        int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                        memoryStream.Close();
                        cryptoStream.Close();
                        return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                    }
                }
            }
        }

        private static string GetUniqueID()
        {
            string cpuInfo = string.Empty;
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                if (cpuInfo == string.Empty)
                {
                    cpuInfo = mo.Properties["processorID"].Value.ToString();
                    break;
                }
            }
            return cpuInfo;
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            byte[] randomBytes = new byte[32];
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
                rngCsp.GetBytes(randomBytes);

            return randomBytes;
        }
    }
}
