using System.Linq;
using System.Security.Cryptography;
using System.Text;
using WinWeelay.Utils;

namespace WinWeelay.Core
{
    public class Sha256Algorithm : BaseHashingAlgorithm
    {
        public override string AlgorithmName => "sha256";

        public override string GenerateHash(string password, string serverNonce, int iterations)
        {
            string salt = serverNonce + GenerateClientNonce();
            byte[] data = HexStringUtils.ConvertHexStringToBytes(salt).Concat(Encoding.UTF8.GetBytes(password)).ToArray();
            using (SHA256 sha256 = new SHA256Managed())
                return GenerateHashParts(AlgorithmName, salt, HexStringUtils.ConvertBytesToHexString(sha256.ComputeHash(data)));
        }
    }
}
