using System.Threading.Tasks;
using MarketingBox.Auth.Service.Domain.Models;

namespace MarketingBox.Auth.Service.Client.Interfaces;

public interface IUserClient
{
    ValueTask<User> GetUser(long userId, string tenantId, bool checkInService = false);
    User GetUser(string emailEncrypted, string tenantId = null);
}