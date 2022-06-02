using System.Runtime.Serialization;

namespace MarketingBox.Auth.Service.Messages
{
    [DataContract]
    public class UserEmailUpdated
    {
        [DataMember(Order = 1)] public long UserId { get; set; }
        [DataMember(Order = 6)] public string Email { get; set; }
        [DataMember(Order = 6)] public string UserName { get; set; }
    }
}