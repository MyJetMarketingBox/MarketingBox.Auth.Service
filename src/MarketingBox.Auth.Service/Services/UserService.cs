using MarketingBox.Auth.Service.Grpc;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Auth.Service.Domain.Models;
using MarketingBox.Auth.Service.Grpc.Models;
using MarketingBox.Auth.Service.Messages;
using MarketingBox.Auth.Service.MyNoSql.Users;
using MarketingBox.Auth.Service.Repositories.Interfaces;
using MarketingBox.Auth.Service.Services.Interfaces;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.Grpc;
using MyJetWallet.Sdk.ServiceBus;

namespace MarketingBox.Auth.Service.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly ICryptoHelper _cryptoHelper;
        private readonly IMyNoSqlServerDataWriter<UserNoSql> _myNoSqlServerDataWriter;
        private readonly IServiceBusPublisher<UserPasswordChangedMessage> _publisherPasswordChanged;
        
        private async Task PublishChangedPassword(User user)
        {
            var message = new UserPasswordChangedMessage
            {
                Email = _cryptoHelper.DecryptEmail(user.EmailEncrypted),
                UserName = user.Username
            };

            await _publisherPasswordChanged.PublishAsync(message);

            await _myNoSqlServerDataWriter.InsertOrReplaceAsync(UserNoSql.Create(user));
        }
        
        public UserService(ILogger<UserService> logger,
            IMyNoSqlServerDataWriter<UserNoSql> myNoSqlServerDataWriter,
            IServiceBusPublisher<UserPasswordChangedMessage> publisherPasswordChanged, 
            IUserRepository userRepository, ICryptoHelper cryptoHelper)
        {
            _logger = logger;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _publisherPasswordChanged = publisherPasswordChanged;
            _userRepository = userRepository;
            _cryptoHelper = cryptoHelper;
        }

        public async Task<Response<User>> CreateAsync(UpsertUserRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new User {@Request}", request);
                var user = await _userRepository.CreateAsync(request);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(UserNoSql.Create(user));
                return MapToResponse(user);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during user creation. {@Request}", request);
                return e.FailedResponse<User>();
            }
        }

        public async Task<Response<User>> UpdateAsync(UpsertUserRequest request)
        {
            try
            {
                _logger.LogInformation("Updating User {@Request}", request);

                var userEntity = await _userRepository.UpdateAsync(request);
                
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(UserNoSql.Create(userEntity));
                return MapToResponse(userEntity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during user update. {@Request}", request);
                return e.FailedResponse<User>();
            }
        }

        public async Task<Response<User>> ChangePasswordAsync(ChangePasswordRequest request)
        {
            try
            {
                var user = await _userRepository.ChangePasswordAsync(request);

                await PublishChangedPassword(user);

                return MapToResponse(user);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during change password. {@Request}", request);
                return e.FailedResponse<User>();
            }
        }

        public async Task<Response<User>> ForceChangePasswordAsync(ForceChangePasswordRequest request)
        {
            try
            {
                var user = await _userRepository.ForceChangePasswordAsync(request);

                await PublishChangedPassword(user);

                return MapToResponse(user);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during change password. {@Request}", request);
                return e.FailedResponse<User>();
            }
        }

        public async Task<Response<IReadOnlyCollection<User>>> SearchAsync(SearchUserRequest request)
        {
            try
            {
                var (userEntities, total) = await _userRepository.SearchAsync(request);

                return new Response<IReadOnlyCollection<User>>
                {
                    Status = ResponseStatus.Ok,
                    Data = userEntities,
                    Total = total
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during user search. {@Request}", request);

                return e.FailedResponse<IReadOnlyCollection<User>>();
            }
        }

        public async Task<Response<User>> GetAsync(GetUserRequest request)
        {
            try
            {
                var user = await _userRepository.GetAsync(request);
                
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(UserNoSql.Create(user));

                return MapToResponse(user);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during user get. {@Request}", request);

                return e.FailedResponse<User>();
            }
        }

        public async Task<Response<bool>> DeleteAsync(DeleteUserRequest request)
        {
            try
            {
                var userRemoved = await _userRepository.DeleteAsync(request);
                await _myNoSqlServerDataWriter.DeleteAsync(UserNoSql.GeneratePartitionKey(userRemoved.TenantId),
                    UserNoSql.GenerateRowKey(userRemoved.EmailEncrypted));
                return new Response<bool>
                {
                    Status = ResponseStatus.Ok,
                    Data = true
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during user delete. {@Request}", request);

                return e.FailedResponse<bool>();
            }
        }

        private static Response<User> MapToResponse(User userEntity)
        {
            return new Response<User>()
            {
                Status = ResponseStatus.Ok,
                Data = userEntity
            };
        }
    }
}