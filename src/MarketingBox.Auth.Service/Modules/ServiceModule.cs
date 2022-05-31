using Autofac;
using MarketingBox.Auth.Service.Engines;
using MarketingBox.Auth.Service.Grpc;
using MarketingBox.Auth.Service.Repositories;
using MarketingBox.Auth.Service.Repositories.Interfaces;
using MarketingBox.Auth.Service.Services;
using MarketingBox.Auth.Service.Services.Interfaces;
using MarketingBox.Auth.Service.Subscribers;
using MarketingBox.Sdk.Crypto;

namespace MarketingBox.Auth.Service.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserRepository>()
                .As<IUserRepository>()
                .SingleInstance();
            builder.RegisterType<UserService>()
                .As<IUserService>()
                .SingleInstance();
            builder.RegisterType<CryptoService>()
                .As<ICryptoService>()
                .SingleInstance();
            builder.RegisterType<CryptoHelper>()
                .As<ICryptoHelper>()
                .SingleInstance();
            builder
                .RegisterType<ResetPasswordSubscriber>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance();
            builder
                .RegisterType<ResetPasswordEngine>()
                .AsSelf()
                .SingleInstance();
        }
    }
}
