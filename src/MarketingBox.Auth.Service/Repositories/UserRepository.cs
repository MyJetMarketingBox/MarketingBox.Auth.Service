using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Auth.Service.Domain.Models;
using MarketingBox.Auth.Service.Grpc.Models;
using MarketingBox.Auth.Service.Postgres;
using MarketingBox.Auth.Service.Repositories.Interfaces;
using MarketingBox.Auth.Service.Services.Interfaces;
using MarketingBox.Sdk.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace MarketingBox.Auth.Service.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly ICryptoHelper _cryptoHelper;


        private static async Task<User> GetUserAsync(string tenantId, string userId, DatabaseContext ctx)
        {
            var userEntity = await ctx.Users.FirstOrDefaultAsync(x =>
                x.TenantId == tenantId &&
                x.ExternalUserId == userId);

            if (userEntity == null)
                throw new NotFoundException("User with id", userId);
            return userEntity;
        }

        public UserRepository(
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            ICryptoHelper cryptoHelper)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _cryptoHelper = cryptoHelper;
        }

        private async Task SaveNewPassword(ForceChangePasswordRequest request, User userEntity, DatabaseContext ctx)
        {
            var (salt, passwordHash) = _cryptoHelper.EncryptPassword(request.NewPassword);
            userEntity.Salt = salt;
            userEntity.PasswordHash = passwordHash;

            ctx.UserLogs.Add(new UserLog
            {
                ChangeType = ChangeType.PasswordChanged,
                ModifiedAt = DateTime.UtcNow,
                TenantId = request.TenantId,
                ModifiedByUserId = request.ChangedByUserId,
                ModifiedForUserId = request.UserId
            });

            await ctx.SaveChangesAsync();
        }

        public async Task<User> CreateAsync(CreateUserRequest request)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var encryptedEmail = _cryptoHelper.EncryptEmail(request.Email);

            var (salt, passwordHash) = _cryptoHelper.EncryptPassword(request.Password);

            var userEntity = new User()
            {
                ExternalUserId = request.ExternalUserId,
                EmailEncrypted = encryptedEmail,
                PasswordHash = passwordHash,
                Salt = salt,
                TenantId = request.TenantId,
                Username = request.Username
            };

            ctx.Users.Add(userEntity);
            try
            {
                await ctx.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
                when (exception.InnerException is PostgresException pgException &&
                      pgException.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                throw new BadRequestException("User already exists.");
            }

            return userEntity;
        }

        public async Task<User> UpdateAsync(UpdateUserRequest request)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var userEntity = await GetUserAsync(request.TenantId, request.ExternalUserId, ctx);
            var encryptedEmail = _cryptoHelper.EncryptEmail(request.Email);

            userEntity.ExternalUserId = request.ExternalUserId;
            userEntity.EmailEncrypted = encryptedEmail;
            userEntity.TenantId = request.TenantId;
            userEntity.Username = request.Username;

            await ctx.SaveChangesAsync();
            return userEntity;
        }

        public async Task<User> ChangePasswordAsync(ChangePasswordRequest request)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var userEntity = await GetUserAsync(request.TenantId, request.UserId.ToString(), ctx);

            _cryptoHelper.ValidatePassword(userEntity.PasswordHash, userEntity.Salt, request.OldPassword);

            await SaveNewPassword(new()
            {
                NewPassword = request.NewPassword,
                TenantId = request.TenantId,
                UserId = request.UserId,
                ChangedByUserId = request.UserId
            }, userEntity, ctx);

            return userEntity;
        }

        public async Task<User> ForceChangePasswordAsync(ForceChangePasswordRequest request)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var userEntity = await GetUserAsync(request.TenantId, request.UserId.ToString(), ctx);
            await SaveNewPassword(request, userEntity, ctx);
            return userEntity;
        }

        public async Task<(IReadOnlyCollection<User>, int)> SearchAsync(SearchUserRequest request)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var query = ctx.Users.AsQueryable();

            if (!string.IsNullOrEmpty(request.TenantId))
            {
                query = query.Where(x => x.TenantId.Equals(request.TenantId));
            }

            if (!string.IsNullOrEmpty(request.Email))
            {
                var encryptedEmail = _cryptoHelper.EncryptEmail(request.Email);

                query = query.Where(x => x.EmailEncrypted.Equals(encryptedEmail));
            }

            if (!string.IsNullOrEmpty(request.Username))
            {
                query = query.Where(x => x.Username.Equals(request.Username));
            }

            if (!string.IsNullOrEmpty(request.ExternalUserId))
            {
                query = query.Where(x => x.ExternalUserId.Equals(request.ExternalUserId));
            }

            var userEntities = await query.ToArrayAsync();
            var total = userEntities.Length;
            return (userEntities, total);
        }

        public async Task<User> GetAsync(GetUserRequest request)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var userEntity = await GetUserAsync(request.TenantId, request.ExternalUserId, ctx);
            return userEntity;
        }

        public async Task<User> DeleteAsync(DeleteUserRequest request)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var userEntity = await GetUserAsync(request.TenantId, request.ExternalUserId, ctx);

            ctx.Users.Remove(userEntity);
            await ctx.SaveChangesAsync();

            return userEntity;
        }
    }
}