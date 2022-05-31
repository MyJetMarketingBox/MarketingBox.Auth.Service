using MarketingBox.Auth.Service.Domain.Models;
using MarketingBox.Auth.Service.Services.Interfaces;
using MarketingBox.Auth.Service.Settings;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Crypto;

namespace MarketingBox.Auth.Service.Services
{
    public class CryptoHelper : ICryptoHelper
    {
        private readonly ICryptoService _cryptoService;
        private readonly SettingsModel _settingsModel;

        public CryptoHelper(ICryptoService cryptoService, SettingsModel settingsModel)
        {
            _cryptoService = cryptoService;
            _settingsModel = settingsModel;
        }

        public (string,string) EncryptPassword(string password)
        {
            var salt = _cryptoService.GenerateSalt();
            var passwordHash = _cryptoService.HashPassword(salt, password);
            return (salt, passwordHash);
        }

        public void ValidatePassword(string passwordHash, string salt, string oldPassword)
        {
            var passwordDecrypted = _cryptoService.Decrypt(
                passwordHash,
                salt,
                Program.Settings.EncryptionSecret);
            if (!passwordDecrypted.Equals(oldPassword))
            {
                throw new ForbiddenException("Old password is wrong.");
            }
        }

        public string DecryptEmail(string encryptedEmail)
        {
            var email = _cryptoService.Decrypt(
                encryptedEmail,
                _settingsModel.EncryptionSalt,
                _settingsModel.EncryptionSecret);
            return email;
        }

        public string EncryptEmail(string decryptedEmail)
        {
            var email = _cryptoService.Encrypt(
                decryptedEmail,
                _settingsModel.EncryptionSalt,
                _settingsModel.EncryptionSecret);
            return email;
        }
    }
}