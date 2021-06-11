using System;
using System.Collections.Generic;
using System.Linq;
using WinWeelay.Utils;

namespace WinWeelay.Core
{
    public class HashFactory
    {
        private List<IHashingAlgorithm> _hashingAlgorithms;

        public HashFactory()
        {
            _hashingAlgorithms = new List<IHashingAlgorithm>();
            _hashingAlgorithms.Add(new Pbkdf2Sha512Algorithm());
            _hashingAlgorithms.Add(new Pbkdf2Sha256Algorithm());
            _hashingAlgorithms.Add(new Sha512Algorithm());
            _hashingAlgorithms.Add(new Sha256Algorithm());
            _hashingAlgorithms.Add(new PlainAlgorithm());
        }

        public string GetSupportedAlgorithms()
        {
            return string.Join(":", _hashingAlgorithms.Select(x => x.AlgorithmName));
        }

        public string GetLegacyInitCommand(string password)
        {
            return $"password={Cipher.Decrypt(password)}";
        }

        public string GetInitCommand(string password, WeechatHashtable handshakeResult)
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
