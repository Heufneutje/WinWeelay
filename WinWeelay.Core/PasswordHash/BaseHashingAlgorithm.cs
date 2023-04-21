using System;
using WinWeelay.Utils;

namespace WinWeelay.Core
{
    /// <summary>
    /// Base class for password hashing.
    /// </summary>
    public abstract class BaseHashingAlgorithm : IHashingAlgorithm
    {
        /// <summary>
        /// The name of the hashing algorithm.
        /// </summary>
        public abstract string AlgorithmName { get; }

        /// <summary>
        /// Create a hash using a given password and the nonce and number of iterations obtained from the relay.
        /// </summary>
        /// <param name="password">The password for the relay connection.</param>
        /// <param name="serverNonce">The nonce obtained from the relay.</param>
        /// <param name="iterations">The number of iterations obtained from the relay.</param>
        /// <returns>A hash of the given password.</returns>
        public abstract string GenerateHash(string password, string serverNonce, int iterations);

        /// <summary>
        /// Generate a random client nonce to append to the server nonce.
        /// </summary>
        /// <param name="size">The size of the random nonce.</param>
        /// <returns>A random nonce.</returns>
        protected string GenerateClientNonce(int size = 16)
        {
            Random random = new();
            byte[] nonceBytes = new byte[size];
            random.NextBytes(nonceBytes);
            return HexStringUtils.ConvertBytesToHexString(nonceBytes);
        }

        /// <summary>
        /// Generate a hash string to be passed to the relay.
        /// </summary>
        /// <param name="algorithm">The algorithm that was used for the hash.</param>
        /// <param name="salt">The used salt.</param>
        /// <param name="hash"></param>
        /// <returns>A hash string that can be passed to the relay directly.</returns>
        protected string GenerateHashParts(string algorithm, string salt, string hash)
        {
            return $"{algorithm}:{salt}:{hash}";
        }

        /// <summary>
        /// Generate a hash string to be passed to the relay.
        /// </summary>
        /// <param name="algorithm">The algorithm that was used for the hash.</param>
        /// <param name="salt">The used salt.</param>
        /// <param name="iterations">The number of interations that was used.</param>
        /// <param name="hash"></param>
        /// <returns>A hash string that can be passed to the relay directly.</returns>
        protected string GenerateHashParts(string algorithm, string salt, int iterations, string hash)
        {
            return $"{algorithm}:{salt}:{iterations}:{hash}";
        }
    }
}
