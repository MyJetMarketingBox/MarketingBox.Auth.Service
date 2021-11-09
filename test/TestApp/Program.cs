using MarketingBox.Auth.Service.Client;
using MarketingBox.Auth.Service.Crypto;
using MarketingBox.Auth.Service.Grpc.Models.Users.Requests;
using ProtoBuf.Grpc.Client;
using System;
using System.Threading.Tasks;
using MarketingBox.Auth.Service.Domain.Models.Users;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();

            var factory = new AuthServiceClientFactory("http://localhost:12347");
            var client = factory.GetUserService();

            var password = "qwerty_123456";
            var email = "general-manager@gmail.com";
            var email2 = "some-email2@gmail.com";

            var resp = await  client.CreateAsync(new CreateUserRequest()
            {
                ExternalUserId = "GeneralManager",
                Email = email,
                Password = password,
                TenantId = "default-tenant-id",
                Username = "GeneralUser",
                Role = UserRole.Admin
            });

            var updResponse = await client.UpdateAsync(new UpdateUserRequest()
            {
                ExternalUserId = "GeneralManager",
                Email = email,
                Password = password,
                TenantId = "default-tenant-id",
                Username = "SomeUser",
                Role = UserRole.Admin
            });

            var get1 = await client.GetAsync(new GetUserRequest()
            {
                ExternalUserId = "GeneralManager",
                TenantId = "default-tenant-id",
            });

            var get2 = await client.GetAsync(new GetUserRequest()
            {
                Email = email2,
                TenantId = "default-tenant-id",
            });

            var get3 = await client.GetAsync(new GetUserRequest()
            {
                TenantId = "default-tenant-id",
                Username = "SomeUser"
            });

            var del = await client.DeleteAsync(new DeleteUserRequest()
            {
                TenantId = "default-tenant-id",
                ExternalUserId = "GeneralManager"
            });

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
