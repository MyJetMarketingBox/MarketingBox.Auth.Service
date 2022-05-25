using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Destructurama.Attributed;
using MarketingBox.Auth.Service.Domain.Models;
using MarketingBox.Auth.Service.Grpc.Models;
using MarketingBox.Sdk.Common.Models.Grpc;

namespace MarketingBox.Auth.Service.Grpc
{
    [ServiceContract]
    public interface IUserService
    {
        [OperationContract]
        Task<Response<User>> CreateAsync(UpsertUserRequest request);
        
        [OperationContract]
        Task<Response<User>> UpdateAsync(UpsertUserRequest request);
        [OperationContract]
        Task<Response<User>> ChangePasswordAsync(ChangePasswordRequest request);
        
        [OperationContract]
        Task<Response<IReadOnlyCollection<User>>> SearchAsync(SearchUserRequest request);
        
        [OperationContract]
        Task<Response<User>> GetAsync(GetUserRequest request);
        
        [OperationContract]
        Task<Response<bool>> DeleteAsync(DeleteUserRequest request);
    }
}
