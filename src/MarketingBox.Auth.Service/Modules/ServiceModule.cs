using Autofac;
using MarketingBox.Auth.Service.Messages;
using MarketingBox.Auth.Service.MyNoSql.Users;
using MarketingBox.Sdk.Crypto;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.ServiceBus;

namespace MarketingBox.Auth.Service.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var serviceBusClient = builder
                .RegisterMyServiceBusTcpClient(
                    Program.ReloadedSettings(e => e.MarketingBoxServiceBusHostPort),
                    Program.LogFactory);
            builder.RegisterMyServiceBusPublisher<UserPasswordChangedMessage>(
                serviceBusClient,
                UserPasswordChangedMessage.Topic,
                false);

            builder.Register(x => new CryptoService())
                .As<ICryptoService>()
                .SingleInstance();

            builder.CreateNoSqlClient(Program.ReloadedSettings(e => e.MyNoSqlReaderHostPort));

            #region Users

            // register writer (IMyNoSqlServerDataWriter<PartnerNoSql>)
            builder.RegisterMyNoSqlWriter<UserNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), UserNoSql.TableName);

            #endregion
        }
    }
}
