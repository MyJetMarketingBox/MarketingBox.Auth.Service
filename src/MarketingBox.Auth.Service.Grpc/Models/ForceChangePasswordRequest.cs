using System.Runtime.Serialization;

namespace MarketingBox.Auth.Service.Grpc.Models;

[DataContract]
public class ForceChangePasswordRequest
{
    [DataMember(Order = 1)]
    public string NewPassword { get; set; }
    
    [DataMember(Order = 2)]
    public string TenantId { get; set; }
    
    [DataMember(Order = 3)]
    public long UserId { get; set; }
    
    [DataMember(Order = 4)]
    public long ChangedByUserId { get; set; }
}