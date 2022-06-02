using System.Runtime.Serialization;

namespace MarketingBox.Auth.Service.Grpc.Models;

[DataContract]
public class CreateUserRequest
{
    [DataMember(Order = 1)]
    public string TenantId { get; set; }

    [DataMember(Order = 2)]
    public string Email { get; set; }

    [DataMember(Order = 3)]
    public string Username { get; set; }
        
    [DataMember(Order = 4)]
    public string Password { get; set; }

    [DataMember(Order = 5)]
    public string ExternalUserId { get; set; }
}