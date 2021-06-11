namespace WinWeelay.Core
{
    public class PlainAlgorithm : BaseHashingAlgorithm
    {
        public override string AlgorithmName => "plain";

        public override string GenerateHash(string password, string serverNonce, int iterations)
        {
            return password;
        }
    }
}
