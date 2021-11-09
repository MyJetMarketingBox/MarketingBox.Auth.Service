using MarketingBox.Auth.Service.Domain.Users;

namespace MarketingBox.Auth.Service.Postgre.Entities.Users
{
    public class UserEntity
    {
        public string TenantId { get; set; }

        public string EmailEncrypted { get; set; }

        public string Username { get; set; }

        public string ExternalUserId { get; set; }

        public string Salt { get; set; }

        public string PasswordHash { get; set; }

        public UserRole Role { get; set; }
    }
}
