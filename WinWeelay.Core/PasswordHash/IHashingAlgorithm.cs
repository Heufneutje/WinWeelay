namespace WinWeelay.Core
{
    public interface IHashingAlgorithm
    {
        string AlgorithmName { get; }
        string GenerateHash(string password, string serverNonce, int iterations);
    }
}
