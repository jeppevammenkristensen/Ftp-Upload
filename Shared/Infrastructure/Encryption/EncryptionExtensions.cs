namespace Shared.Infrastructure.Encryption
{
    public static class EncryptionExtensions
    {

        public static string Decrypt(this string cipherText)
        {
            var crypt = new PasswordCrypt();
            return crypt.Decrypt(cipherText);
        }

        public static string Encrypt(this string cipherText)
        {
            var crypt = new PasswordCrypt();
            return crypt.Encrypt(cipherText);
        }
    }
}