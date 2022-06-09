using System;

namespace MarketingBox.Auth.Service.Domain.Models;

public class UserLog
{
    public long Id { get; set; }
    public DateTime ModifiedAt { get; set; }
    public ChangeType ChangeType { get; set; }
    public long ModifiedForUserId { get; set; }
    public long ModifiedByUserId { get; set; }
    public string TenantId { get; set; }
}

public enum ChangeType
{
    PasswordChanged,
    EmailChanged,
}