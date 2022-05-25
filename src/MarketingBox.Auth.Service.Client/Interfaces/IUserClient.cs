using System.Threading.Tasks;
using MarketingBox.Auth.Service.Domain.Models;

namespace MarketingBox.Auth.Service.Client.Interfaces;

public interface IUserClient
{
    Task<User> GetUser(long userId, string tenantId, bool checkInService = false);
    User GetUser(string tenantId, string emailEncrypted);
}