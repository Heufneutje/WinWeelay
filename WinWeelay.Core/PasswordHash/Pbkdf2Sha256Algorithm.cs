namespace WinWeelay.Core
{
    public class Pbkdf2Sha256Algorithm : Pbkdf2Algorithm
    {
        public override int ShaSize => 256;
    }
}
