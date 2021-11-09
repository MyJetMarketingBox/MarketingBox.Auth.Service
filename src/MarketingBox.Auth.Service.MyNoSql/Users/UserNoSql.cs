using MarketingBox.Auth.Service.Domain.Models.Users;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Auth.Service.MyNoSql.Users
{
    public class UserNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-authservice-users";
        public static string GeneratePartitionKey(string tenantId) => $"{tenantId}";
        public static string GenerateRowKey(string emailEncrypted) =>
            $"{emailEncrypted}";

        public string TenantId { get; set; }

        public string EmailEncrypted { get; set; }

        public string Username { get; set; }
        public string ExternalUserId { get; set; }

        public string Salt { get; set; }

        public string PasswordHash { get; set; }

        public UserRole Role { get; set; }
        

        public static UserNoSql Create(
            string tenantId,
            string emailEncrypted,
            string username,
            string externalUserId,
            string salt,
            string passwordHash,
            UserRole userRole) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(tenantId),
                RowKey = GenerateRowKey(emailEncrypted),
                TenantId = tenantId,
                PasswordHash = passwordHash,
                Salt = salt,
                Username = username,
                EmailEncrypted = emailEncrypted,
                ExternalUserId = externalUserId,
                Role = userRole
            };
    }
}
