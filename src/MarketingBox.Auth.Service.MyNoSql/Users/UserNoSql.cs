using MarketingBox.Auth.Service.Domain.Models;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Auth.Service.MyNoSql.Users
{
    public class UserNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-users";
        public static string GeneratePartitionKey(string tenantId) => $"{tenantId}";
        public static string GenerateRowKey(string emailEncrypted) =>
            $"{emailEncrypted}";

        public User User { get; set; }
        

        public static UserNoSql Create(User user) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(user.TenantId),
                RowKey = GenerateRowKey(user.EmailEncrypted),
                User = user
            };
    }
}
