using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Auth.Service.Grpc.Models;
using MarketingBox.Sdk.Common.Models.Grpc;

namespace MarketingBox.Auth.Service.Grpc
{
    [ServiceContract]
    public interface ITokensService
    {
        [OperationContract]
        Task<Response<TokenInfo>> LoginAsync(TokenRequest request);
    }
}