using System.Collections.Generic;
using MarketingBox.Auth.Service.Domain.Models;
using MarketingBox.Auth.Service.Grpc.Models;
using MarketingBox.Auth.Service.Services.Interfaces;
using MarketingBox.Auth.Service.Settings;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Models;
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
            var passwordEncrypted = _cryptoService.HashPassword(
                salt,
                oldPassword);
            if (passwordEncrypted.Equals(passwordHash)) return;
            var errorMessage = "Password is wrong";
            throw new BadRequestException(new Error
            {
                ErrorMessage = errorMessage,
                ValidationErrors = new List<ValidationError>
                {
                    new()
                    {
                        ErrorMessage = errorMessage,
                        ParameterName = nameof(ChangePasswordRequest.OldPassword)
                    }
                }
            });
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