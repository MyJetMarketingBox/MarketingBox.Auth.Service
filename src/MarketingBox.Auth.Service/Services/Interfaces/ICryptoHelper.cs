namespace MarketingBox.Auth.Service.Services.Interfaces
{
    public interface ICryptoHelper
    {
        (string, string) EncryptPassword(string password);
        void ValidatePassword(string passwordHash, string salt, string oldPassword);
        string DecryptEmail(string encryptedEmail);
        string EncryptEmail(string decryptedEmail);
    }
}