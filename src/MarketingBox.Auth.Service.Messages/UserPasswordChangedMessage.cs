using System.Runtime.Serialization;

namespace MarketingBox.Auth.Service.Messages;

[DataContract]
public class UserPasswordChangedMessage
{
    public const string Topic = "marketing-box-auth-service-user-email";

    [DataMember(Order = 1)] public string Email { get; set; }
    [DataMember(Order = 2)] public string UserName { get; set; }
}