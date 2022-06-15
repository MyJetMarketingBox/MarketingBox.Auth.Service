using System;
using System.Runtime.Serialization;

namespace MarketingBox.Auth.Service.Grpc.Models
{
    [DataContract]
    public class TokenInfo
    {
        [DataMember(Order = 1)]
        public string Token { get; set; }

        [DataMember(Order = 2)]
        public DateTime ExpiresAt { get; set; }
    }
}