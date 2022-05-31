using Autofac;
using MarketingBox.Auth.Service.Messages;
using MarketingBox.Auth.Service.MyNoSql.Users;
using MarketingBox.PasswordApi.Domain.Models;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.Abstractions;

namespace MarketingBox.Auth.Service.Modules
{
    public class ClientModule : Module
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
            builder.RegisterMyServiceBusSubscriberBatch<PasswordResetMessage>(
                serviceBusClient, PasswordResetMessage.Topic, 
                "MarketingBox-Email-Service-password-reset",
                TopicQueueType.PermanentWithSingleConnection);

            builder.CreateNoSqlClient(Program.ReloadedSettings(e => e.MyNoSqlReaderHostPort));

            // register writer (IMyNoSqlServerDataWriter<PartnerNoSql>)
            builder.RegisterMyNoSqlWriter<UserNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), UserNoSql.TableName);
        }
    }
}
