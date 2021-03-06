﻿namespace WinWeelay.Core
{
    /// <summary>
    /// Handler for returning the plain password.
    /// </summary>
    public class PlainAlgorithm : BaseHashingAlgorithm
    {
        /// <summary>
        /// The name of the hashing algorithm.
        /// </summary>
        public override string AlgorithmName => "plain";

        /// <summary>
        /// Create a hash using a given password and the nonce and number of iterations obtained from the relay.
        /// </summary>
        /// <param name="password">The password for the relay connection.</param>
        /// <param name="serverNonce">The nonce obtained from the relay.</param>
        /// <param name="iterations">The number of iterations obtained from the relay.</param>
        /// <returns>A hash of the given password.</returns>
        /// <returns></returns>
        public override string GenerateHash(string password, string serverNonce, int iterations)
        {
            return password;
        }
    }
}
