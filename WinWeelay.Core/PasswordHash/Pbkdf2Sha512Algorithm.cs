namespace WinWeelay.Core
{
    /// <summary>
    /// Handler for creating PBKDF2+SHA512 hashes.
    /// </summary>
    public class Pbkdf2Sha512Algorithm : Pbkdf2Algorithm
    {
        /// <summary>
        /// The size of SHA hash to use.
        /// </summary>
        public override int ShaSize => 512;
    }
}
