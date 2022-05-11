using System.Threading.Tasks;
using MarketingBox.Auth.Service.Domain.Models;

namespace MarketingBox.Auth.Service.Client.Interfaces;

public interface IUserClient
{
    Task<User> GetUser(string tenantId, long userId);
}