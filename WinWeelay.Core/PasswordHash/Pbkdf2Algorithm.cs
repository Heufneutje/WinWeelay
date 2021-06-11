using System.Security.Cryptography;
using WinWeelay.Utils;

namespace WinWeelay.Core
{
    public abstract class Pbkdf2Algorithm : BaseHashingAlgorithm
    {
        public abstract int ShaSize { get; }
        public override string AlgorithmName => $"pbkdf2+sha{ShaSize}";

        public override string GenerateHash(string password, string serverNonce, int iterations)
        {
            HashAlgorithmName hashAlgorithm = (HashAlgorithmName)typeof(HashAlgorithmName).GetProperty($"SHA{ShaSize}").GetValue(null, null);
            string salt = serverNonce + GenerateClientNonce();
            using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, HexStringUtils.ConvertHexStringToBytes(salt), iterations, hashAlgorithm))
                return GenerateHashParts(AlgorithmName, salt, iterations, HexStringUtils.ConvertBytesToHexString(rfc2898DeriveBytes.GetBytes(ShaSize / 8)));
        }
    }
}
