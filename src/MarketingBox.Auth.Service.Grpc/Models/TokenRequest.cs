using System.Runtime.Serialization;

namespace MarketingBox.Auth.Service.Grpc.Models
{
    [DataContract]
    public class TokenRequest
    {
        [DataMember(Order = 1)] public string Login { get; set; }
        [DataMember(Order = 2)] public string TenantId { get; set; }
        [DataMember(Order = 3)] public string Password { get; set; }
    }
}