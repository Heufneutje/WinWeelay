namespace WinWeelay.Core
{
    /// <summary>
    /// Handler for creating PBKDF2+SHA256 hashes.
    /// </summary>
    public class Pbkdf2Sha256Algorithm : Pbkdf2Algorithm
    {
        /// <summary>
        /// The size of SHA hash to use.
        /// </summary>
        public override int ShaSize => 256;
    }
}
