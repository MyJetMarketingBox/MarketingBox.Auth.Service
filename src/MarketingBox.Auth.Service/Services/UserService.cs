using MarketingBox.Auth.Service.Grpc;
using MarketingBox.Auth.Service.Grpc.Models.Users;
using MarketingBox.Auth.Service.Grpc.Models.Users.Requests;
using MarketingBox.Auth.Service.Messages.Users;
using MarketingBox.Auth.Service.Postgre;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Auth.Service.Crypto;
using MarketingBox.Auth.Service.Domain;
using MarketingBox.Auth.Service.Domain.Users;
using MarketingBox.Auth.Service.MyNoSql.Users;
using MarketingBox.Auth.Service.Postgre.Entities.Users;
using MarketingBox.Auth.Service.Settings;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models;
using MarketingBox.Sdk.Common.Models.Grpc;
using MyJetWallet.Sdk.ServiceBus;
using Npgsql;

namespace MarketingBox.Auth.Service.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IMyNoSqlServerDataWriter<UserNoSql> _myNoSqlServerDataWriter;
        private readonly IServiceBusPublisher<UserUpdated> _publisherUserUpdated;
        private readonly IServiceBusPublisher<UserRemoved> _publisherUserRemoved;
        private readonly ICryptoService _cryptoService;
        private readonly SettingsModel _settingsModel;

        public UserService(ILogger<UserService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IMyNoSqlServerDataWriter<UserNoSql> myNoSqlServerDataWriter,
            IServiceBusPublisher<UserUpdated> publisherUserUpdated,
            IServiceBusPublisher<UserRemoved> publisherUserRemoved,
            ICryptoService cryptoService,
            Settings.SettingsModel settingsModel)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _publisherUserUpdated = publisherUserUpdated;
            _publisherUserRemoved = publisherUserRemoved;
            _cryptoService = cryptoService;
            _settingsModel = settingsModel;
        }

        public async Task<Response<User>> CreateAsync(CreateUserRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new User {@context}", request);
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var encryptedEmail = _cryptoService.Encrypt(
                    request.Email,
                    _settingsModel.EncryptionSalt,
                    _settingsModel.EncryptionSecret);

                var salt = _cryptoService.GenerateSalt();
                var passwordHash = _cryptoService.HashPassword(salt, request.Password);

                var userEntity = new UserEntity()
                {
                    ExternalUserId = request.ExternalUserId,
                    EmailEncrypted = encryptedEmail,
                    PasswordHash = passwordHash,
                    Salt = salt,
                    TenantId = request.TenantId,
                    Username = request.Username,
                    Role = request.Role.MapEnum<UserRole>()
                };

                ctx.Users.Add(userEntity);
                await ctx.SaveChangesAsync();

                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(MapToNosql(userEntity));
                _logger.LogInformation("Created new User in NoSQL {@context}", request);

                await _publisherUserUpdated.PublishAsync(MapToMessage(userEntity));
                _logger.LogInformation("Sent event Created new User {@context}", request);

                return MapToResponse(userEntity);
            }
            catch (DbUpdateException exception)
                when (exception.InnerException is PostgresException pgException &&
                      pgException.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                _logger.LogError(exception, "Error during user creation. {@context}", request);
                return new Response<User>
                {
                    Status = ResponseStatus.BadRequest,
                    Error = new Error
                    {
                        ErrorMessage = "User already exists."
                    }
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during user creation. {@context}", request);
                return e.FailedResponse<User>();
            }
        }

        public async Task<Response<User>> UpdateAsync(UpdateUserRequest request)
        {
            try
            {
                _logger.LogInformation("Updating User {@context}", request);
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var encryptedEmail = _cryptoService.Encrypt(
                    request.Email,
                    _settingsModel.EncryptionSalt,
                    _settingsModel.EncryptionSecret);

                var salt = _cryptoService.GenerateSalt();
                var passwordHash = _cryptoService.HashPassword(salt, request.Password);

                var userEntity = new UserEntity()
                {
                    ExternalUserId = request.ExternalUserId,
                    EmailEncrypted = encryptedEmail,
                    PasswordHash = passwordHash,
                    Salt = salt,
                    TenantId = request.TenantId,
                    Username = request.Username,
                    Role = request.Role.MapEnum<UserRole>()
                };

                ctx.Users.Upsert(userEntity);
                await ctx.SaveChangesAsync();

                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(MapToNosql(userEntity));
                _logger.LogInformation("Updated User in NoSQL {@context}", request);

                await _publisherUserUpdated.PublishAsync(MapToMessage(userEntity));
                _logger.LogInformation("Sent event Updated User {@context}", request);

                return MapToResponse(userEntity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during user update. {@context}", request);

                return e.FailedResponse<User>();
            }
        }

        public async Task<Response<IReadOnlyCollection<User>>> GetAsync(GetUserRequest request)
        {
            try
            {
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var query = ctx.Users.AsQueryable();

                if (!string.IsNullOrEmpty(request.TenantId))
                {
                    query = query.Where(x => x.TenantId == request.TenantId);
                }

                if (!string.IsNullOrEmpty(request.Email))
                {
                    var encryptedEmail = _cryptoService.Encrypt(
                        request.Email,
                        _settingsModel.EncryptionSalt,
                        _settingsModel.EncryptionSecret);

                    query = query.Where(x => x.EmailEncrypted == encryptedEmail);
                }

                if (!string.IsNullOrEmpty(request.Username))
                {
                    query = query.Where(x => x.Username == request.Username);
                }

                if (!string.IsNullOrEmpty(request.ExternalUserId))
                {
                    query = query.Where(x => x.ExternalUserId == request.ExternalUserId);
                }

                var userEntity = await query.ToArrayAsync();

                if (userEntity.Length == 0)
                {
                    throw new NotFoundException(NotFoundException.DefaultMessage);
                }

                return new Response<IReadOnlyCollection<User>>
                {
                    Status = ResponseStatus.Ok,
                    Data = userEntity.Select(Map).ToArray()
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during user get. {@context}", request);

                return e.FailedResponse<IReadOnlyCollection<User>>();
            }
        }

        public async Task<Response<bool>> DeleteAsync(DeleteUserRequest request)
        {
            try
            {
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var userEntity = await ctx.Users.FirstOrDefaultAsync(x =>
                    x.TenantId == request.TenantId &&
                    x.ExternalUserId == request.ExternalUserId);

                if (userEntity == null)
                    throw new NotFoundException(nameof(request.ExternalUserId),request.ExternalUserId);

                await _myNoSqlServerDataWriter.DeleteAsync(UserNoSql.GeneratePartitionKey(userEntity.TenantId),
                    UserNoSql.GenerateRowKey(userEntity.EmailEncrypted));

                await _publisherUserRemoved.PublishAsync(new UserRemoved()
                {
                    Username = userEntity.Username,
                    EmailEncrypted = userEntity.EmailEncrypted,
                    TenantId = userEntity.TenantId
                });

                await ctx.Users
                    .Where(x =>
                        x.TenantId == request.TenantId &&
                        x.ExternalUserId == request.ExternalUserId).DeleteFromQueryAsync();

                return new Response<bool>
                {
                    Status = ResponseStatus.Ok,
                    Data = true
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during user get. {@context}", request);

                return e.FailedResponse<bool>();
            }
        }

        private Response<User> MapToResponse(UserEntity userEntity)
        {
            return new Response<User>()
            {
                Status = ResponseStatus.Ok,
                Data = Map(userEntity)
            };
        }

        private static User Map(UserEntity userEntity)
        {
            return new User()
            {
                Username = userEntity.Username,
                Salt = userEntity.Salt,
                PasswordHash = userEntity.PasswordHash,
                EmailEncrypted = userEntity.EmailEncrypted,
                TenantId = userEntity.TenantId,
                ExternalUserId = userEntity.ExternalUserId,
                Role = userEntity.Role.MapEnum<Domain.Models.Users.UserRole>()
            };
        }

        private UserUpdated MapToMessage(UserEntity userEntity)
        {
            return new UserUpdated()
            {
                Salt = userEntity.Salt,
                PasswordHash = userEntity.PasswordHash,
                Username = userEntity.Username,
                EmailEncrypted = userEntity.EmailEncrypted,
                TenantId = userEntity.TenantId,
                ExternalUserId = userEntity.ExternalUserId,
                Role = userEntity.Role.MapEnum<Domain.Models.Users.UserRole>()
            };
        }

        private UserNoSql MapToNosql(UserEntity userEntity)
        {
            return UserNoSql.Create(
                userEntity.TenantId,
                userEntity.EmailEncrypted,
                userEntity.Username,
                userEntity.ExternalUserId,
                userEntity.Salt,
                userEntity.PasswordHash,
                userEntity.Role.MapEnum<Domain.Models.Users.UserRole>());
        }
    }
}