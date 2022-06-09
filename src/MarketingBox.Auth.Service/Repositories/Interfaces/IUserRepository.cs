using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Auth.Service.Domain.Models;
using MarketingBox.Auth.Service.Grpc.Models;

namespace MarketingBox.Auth.Service.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> CreateAsync(CreateUserRequest request);

        Task<User> UpdateAsync(UpdateUserRequest request);

        Task<User> ChangePasswordAsync(ChangePasswordRequest request);
        
        Task<User> ForceChangePasswordAsync(ForceChangePasswordRequest request);

        Task<(IReadOnlyCollection<User>, int)> SearchAsync(SearchUserRequest request);

        Task<User> GetAsync(GetUserRequest request);

        Task<User> DeleteAsync(DeleteUserRequest request);
    }
}