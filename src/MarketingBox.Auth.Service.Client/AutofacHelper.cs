using Autofac;
using MarketingBox.Auth.Service.Client.Interfaces;
using MarketingBox.Auth.Service.Grpc;
using MarketingBox.Auth.Service.MyNoSql.Users;
using MarketingBox.Sdk.Crypto;
using MyJetWallet.Sdk.NoSql;
using MyNoSqlServer.DataReader;

// ReSharper disable UnusedMember.Global

namespace MarketingBox.Auth.Service.Client
{
    public static class AutofacHelper
    {
        public static void RegisterAuthServiceClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new AuthServiceClientFactory(grpcServiceUrl);
            
            builder.RegisterInstance(factory.GetUserService()).As<IUserService>().SingleInstance();
            builder.RegisterInstance(factory.GetTokenService()).As<ITokensService>().SingleInstance();
        }

        public static void RegisterUserClient(
            this ContainerBuilder builder,
            string grpcServiceUrl,
            IMyNoSqlSubscriber noSqlClient)
        {
            var factory = new AuthServiceClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetUserService()).As<IUserService>().SingleInstance();
            builder.RegisterType<CryptoService>().As<ICryptoService>().SingleInstance();
            builder.RegisterType<UserClient>().As<IUserClient>().SingleInstance();

            builder.RegisterMyNoSqlReader<UserNoSql>(noSqlClient, UserNoSql.TableName);
        }
    }
}