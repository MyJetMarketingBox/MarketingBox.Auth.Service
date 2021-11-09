using System.Runtime.Serialization;
using MarketingBox.Auth.Service.Domain.Models.Users;

namespace MarketingBox.Auth.Service.Grpc.Models.Users
{
    [DataContract]
    public class User
    {
        [DataMember(Order = 1)]
        public string TenantId { get; set; }

        [DataMember(Order = 2)]
        public string EmailEncrypted { get; set; }

        [DataMember(Order = 3)]
        public string Username { get; set; }

        [DataMember(Order = 4)]
        public string Salt { get; set; }

        [DataMember(Order = 5)]
        public string PasswordHash { get; set; }

        [DataMember(Order = 6)]
        public string ExternalUserId { get; set; }

        [DataMember(Order = 7)]
        public UserRole Role { get; set; }

    }
}
