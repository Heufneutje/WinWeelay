using System.Security.Cryptography;
using WinWeelay.Utils;

namespace WinWeelay.Core
{
    /// <summary>
    /// Base class for creating PBKDF2 hashes.
    /// </summary>
    public abstract class Pbkdf2Algorithm : BaseHashingAlgorithm
    {
        /// <summary>
        /// The size of SHA hash to use.
        /// </summary>
        public abstract int ShaSize { get; }

        /// <summary>
        /// The name of the hashing algorithm.
        /// </summary>
        public override string AlgorithmName => $"pbkdf2+sha{ShaSize}";

        /// <summary>
        /// Create a hash using a given password and the nonce and number of iterations obtained from the relay.
        /// </summary>
        /// <param name="password">The password for the relay connection.</param>
        /// <param name="serverNonce">The nonce obtained from the relay.</param>
        /// <param name="iterations">The number of iterations obtained from the relay.</param>
        /// <returns>A hash of the given password.</returns>
        public override string GenerateHash(string password, string serverNonce, int iterations)
        {
            HashAlgorithmName hashAlgorithm = (HashAlgorithmName)typeof(HashAlgorithmName).GetProperty($"SHA{ShaSize}").GetValue(null, null);
            string salt = serverNonce + GenerateClientNonce();
            using (Rfc2898DeriveBytes rfc2898DeriveBytes = new(password, HexStringUtils.ConvertHexStringToBytes(salt), iterations, hashAlgorithm))
                return GenerateHashParts(AlgorithmName, salt, iterations, HexStringUtils.ConvertBytesToHexString(rfc2898DeriveBytes.GetBytes(ShaSize / 8)));
        }
    }
}
