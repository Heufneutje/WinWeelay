using System;
using WinWeelay.Utils;

namespace WinWeelay.Core
{
    public abstract class BaseHashingAlgorithm : IHashingAlgorithm
    {
        public abstract string AlgorithmName { get; }

        public abstract string GenerateHash(string password, string serverNonce, int iterations);

        protected string GenerateClientNonce(int size = 16)
        {
            Random random = new Random();
            byte[] nonceBytes = new byte[size];
            random.NextBytes(nonceBytes);
            return HexStringUtils.ConvertBytesToHexString(nonceBytes);
        }

        protected string GenerateHashParts(string algorithm, string salt, string hash)
        {
            return $"{algorithm}:{salt}:{hash}";
        }

        protected string GenerateHashParts(string algorithm, string salt, int iterations, string hash)
        {
            return $"{algorithm}:{salt}:{iterations}:{hash}";
        }
    }
}
