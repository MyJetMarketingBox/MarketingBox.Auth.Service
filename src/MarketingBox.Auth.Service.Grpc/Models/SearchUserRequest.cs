using System.Runtime.Serialization;
using Destructurama.Attributed;

namespace MarketingBox.Auth.Service.Grpc.Models
{
    [DataContract]
    public class SearchUserRequest
    {
        [DataMember(Order = 1)]
        public string TenantId { get; set; }

        [DataMember(Order = 2)]
        [LogMasked(PreserveLength = false)]
        public string Email { get; set; }

        [DataMember(Order = 3)]
        public string Username { get; set; }

        [DataMember(Order = 4)]
        public string ExternalUserId { get; set; }
    }
}
