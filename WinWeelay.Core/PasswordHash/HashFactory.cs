using System;
using System.Collections.Generic;
using System.Linq;
using WinWeelay.Utils;

namespace WinWeelay.Core
{
    /// <summary>
    /// Factory for hashing a password using the hashing algorithm that's requested by the server.
    /// </summary>
    public class HashFactory
    {
        private List<IHashingAlgorithm> _hashingAlgorithms;

        /// <summary>
        /// Create an instance of the factory.
        /// </summary>
        public HashFactory()
        {
            _hashingAlgorithms = new List<IHashingAlgorithm>();
            _hashingAlgorithms.Add(new Pbkdf2Sha512Algorithm());
            _hashingAlgorithms.Add(new Pbkdf2Sha256Algorithm());
            _hashingAlgorithms.Add(new Sha512Algorithm());
            _hashingAlgorithms.Add(new Sha256Algorithm());
            _hashingAlgorithms.Add(new PlainAlgorithm());
        }

        /// <summary>
        /// Get a formatted list of all the algorithms supported by the client.
        /// </summary>
        /// <returns>A list of algorithm names separated by colons.</returns>
        public string GetSupportedAlgorithms()
        {
            return string.Join(":", _hashingAlgorithms.Select(x => x.AlgorithmName));
        }

        /// <summary>
        /// Get an init command parameter with the password.
        /// </summary>
        /// <param name="password">The encrytped relay password from the config file</param>
        /// <returns>An init command parameter with the password.</returns>
        public string GetLegacyInitCommandParameter(string password)
        {
            return $"password={Cipher.Decrypt(password)}";
        }

        /// <summary>
        /// Get an init command parameter with a password hash.
        /// </summary>
        /// <param name="password">The encrytped relay password from the config file.</param>
        /// <param name="handshakeResult">The handshake parameters obtained from the relay.</param>
        /// <returns>An init command parameter with the password hash.</returns>
        public string GetInitCommandParameter(string password, WeechatHashtable handshakeResult)
        {
            if (handshakeResult["totp"].AsString() == "on")
                throw new NotSupportedException("TOTP is enabled on the relay but not supported by this client.");

            string hashAlgorithmName = handshakeResult["password_hash_algo"].AsString();
            int iterations = Convert.ToInt32(handshakeResult["password_hash_iterations"].AsString());
            string serverNonce = handshakeResult["nonce"].AsString();

            IHashingAlgorithm hashingAlgorithm = _hashingAlgorithms.First(x => x.AlgorithmName == hashAlgorithmName);

            string command = hashAlgorithmName == "plain" ? "password" : "password_hash";
            return $"{command}={hashingAlgorithm.GenerateHash(Cipher.Decrypt(password), serverNonce, iterations)}";
        }
    }
}
