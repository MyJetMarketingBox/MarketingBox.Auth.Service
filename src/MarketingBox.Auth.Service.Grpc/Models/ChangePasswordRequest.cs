using System.Runtime.Serialization;

namespace MarketingBox.Auth.Service.Grpc.Models;

[DataContract]
public class ChangePasswordRequest
{
    [DataMember(Order = 1)]
    public string NewPassword { get; set; }
    
    [DataMember(Order = 2)]
    public string OldPassword { get; set; }
    
    [DataMember(Order = 3)]
    public string TenantId { get; set; }
    
    [DataMember(Order = 4)]
    public long UserId { get; set; }
}