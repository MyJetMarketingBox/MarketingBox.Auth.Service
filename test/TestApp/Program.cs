using MarketingBox.Auth.Service.Client;
using ProtoBuf.Grpc.Client;
using System;
using System.Threading.Tasks;
using MarketingBox.Auth.Service.Grpc.Models;

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
            
            await  client.CreateAsync(new CreateUserRequest()
            {
                ExternalUserId = "Supervisor",
                Email = "supervisor@gmail.com",
                Password = "qwerty_123456",
                TenantId = "default-tenant-id",
                Username = "Supervisor"
            });

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
