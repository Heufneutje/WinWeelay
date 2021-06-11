namespace WinWeelay.Core
{
    public class Pbkdf2Sha512Algorithm : Pbkdf2Algorithm
    {
        public override int ShaSize => 512;
    }
}
