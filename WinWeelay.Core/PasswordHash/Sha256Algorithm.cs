using System.Linq;
using System.Security.Cryptography;
using System.Text;
using WinWeelay.Utils;

namespace WinWeelay.Core
{
    /// <summary>
    /// Handler for creating SHA256 hashes.
    /// </summary>
    public class Sha256Algorithm : BaseHashingAlgorithm
    {
        /// <summary>
        /// The name of the hashing algorithm.
        /// </summary>
        public override string AlgorithmName => "sha256";

        /// <summary>
        /// Create a hash using a given password and the nonce and number of iterations obtained from the relay.
        /// </summary>
        /// <param name="password">The password for the relay connection.</param>
        /// <param name="serverNonce">The nonce obtained from the relay.</param>
        /// <param name="iterations">The number of iterations obtained from the relay.</param>
        /// <returns>A hash of the given password.</returns>
        public override string GenerateHash(string password, string serverNonce, int iterations)
        {
            string salt = serverNonce + GenerateClientNonce();
            byte[] data = HexStringUtils.ConvertHexStringToBytes(salt).Concat(Encoding.UTF8.GetBytes(password)).ToArray();
            using (SHA256 sha256 = new SHA256Managed())
                return GenerateHashParts(AlgorithmName, salt, HexStringUtils.ConvertBytesToHexString(sha256.ComputeHash(data)));
        }
    }
}
