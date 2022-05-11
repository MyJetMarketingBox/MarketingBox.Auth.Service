using System.Runtime.Serialization;

namespace MarketingBox.Auth.Service.Grpc.Models
{
    [DataContract]
    public class DeleteUserRequest
    {
        [DataMember(Order = 1)]
        public string TenantId { get; set; }
        
        [DataMember(Order = 2)]
        public string ExternalUserId { get; set; }
    }
}
