using System.Linq;
using System.Security.Cryptography;
using System.Text;
using WinWeelay.Utils;

namespace WinWeelay.Core
{
    public class Sha512Algorithm : BaseHashingAlgorithm
    {
        public override string AlgorithmName => "sha512";

        public override string GenerateHash(string password, string serverNonce, int iterations)
        {
            string salt = serverNonce + GenerateClientNonce();
            byte[] data = HexStringUtils.ConvertHexStringToBytes(salt).Concat(Encoding.UTF8.GetBytes(password)).ToArray();
            using (SHA512 sha512 = new SHA512Managed())
                return GenerateHashParts(AlgorithmName, salt, HexStringUtils.ConvertBytesToHexString(sha512.ComputeHash(data)));
        }
    }
}
