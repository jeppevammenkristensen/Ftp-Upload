using System;
using ConfigR;
using Effortless.Net.Encryption;
using Upload.Configuration;

namespace Upload.Infrastructure
{
    public class PasswordCrypt
    {
        private static Lazy<EncryptionConfiguration> _configuration = new Lazy<EncryptionConfiguration>(() => Config.Global.Get<EncryptionConfiguration>());

        public string Decrypt(string cipherText)
        {
            return Strings.Decrypt(cipherText, _configuration.Value.Key, _configuration.Value.IV);
        }

        public string Encrypt(string password)
        {
            return Strings.Encrypt(password, _configuration.Value.Key, _configuration.Value.IV);
        }
    }
}