namespace WinWeelay.Core
{
    /// <summary>
    /// Interface for password hashing.
    /// </summary>
    public interface IHashingAlgorithm
    {
        /// <summary>
        /// The name of the hashing algorithm.
        /// </summary>
        string AlgorithmName { get; }

        /// <summary>
        /// Create a hash using a given password and the nonce and number of iterations obtained from the relay.
        /// </summary>
        /// <param name="password">The password for the relay connection.</param>
        /// <param name="serverNonce">The nonce obtained from the relay.</param>
        /// <param name="iterations">The number of iterations obtained from the relay.</param>
        /// <returns>A hash of the given password.</returns>
        string GenerateHash(string password, string serverNonce, int iterations);
    }
}
