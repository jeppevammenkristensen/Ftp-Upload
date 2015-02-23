using System;
using ConfigR;
using Effortless.Net.Encryption;
using Upload.Configuration;

namespace Upload.Infrastructure.Encryption
{
    public class PasswordCrypt
    {
        private static readonly Lazy<EncryptionConfiguration> _configuration = new Lazy<EncryptionConfiguration>(() => Config.Global.Get<EncryptionConfiguration>(ConfigurationKeys.Encryption));

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