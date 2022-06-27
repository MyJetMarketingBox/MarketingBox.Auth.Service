using JetBrains.Annotations;
using MarketingBox.Auth.Service.Grpc;
using MyJetWallet.Sdk.Grpc;

namespace MarketingBox.Auth.Service.Client
{
    [UsedImplicitly]
    public class AuthServiceClientFactory: MyGrpcClientFactory
    {
        public AuthServiceClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }

        public IUserService GetUserService() => CreateGrpcService<IUserService>();
        public ITokensService GetTokenService() => CreateGrpcService<ITokensService>();
    }
}
