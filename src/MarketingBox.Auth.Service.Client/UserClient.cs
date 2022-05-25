using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Auth.Service.Client.Interfaces;
using MarketingBox.Auth.Service.Domain.Models;
using MarketingBox.Auth.Service.Grpc;
using MarketingBox.Auth.Service.MyNoSql.Users;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Extensions;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Auth.Service.Client;

public class UserClient : IUserClient
{
    private readonly IUserService _userService;
    private readonly IMyNoSqlServerDataReader<UserNoSql> _noSqlReader;
    private readonly ILogger<UserClient> _logger;

    public UserClient(IUserService userService,
        IMyNoSqlServerDataReader<UserNoSql> noSqlReader,
        ILogger<UserClient> logger)
    {
        _userService = userService;
        _noSqlReader = noSqlReader;
        _logger = logger;
    }

    public async Task<User> GetUser(long userId, string tenantId, bool checkInService = false)
    {
        try
        {
            _logger.LogInformation("Getting user from nosql server.");
            var user = _noSqlReader.Get(tenantId, x => x.User.ExternalUserId == userId.ToString())
                .FirstOrDefault()?.User;

            if (user != null)
            {
                return user;
            }

            if (checkInService)
            {
                _logger.LogInformation("Getting user from grpc service.");
                var result = await _userService.GetAsync(new()
                {
                    TenantId = tenantId,
                    ExternalUserId = userId.ToString()
                });
                user = result.Process();
                return user;
            }

            if (user is null)
                throw new NotFoundException("User with id", userId);

            return user;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured while getting user.");
            throw;
        }
    }

    public User GetUser(string tenantId, string emailEncrypted)
    {
        try
        {
            _logger.LogInformation("Getting user from nosql server.");
            var user = _noSqlReader.Get(tenantId, emailEncrypted)?.User;

            if (user != null)
            {
                return user;
            }

            throw new NotFoundException("User with such email was not found.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured while getting user.");
            throw;
        }
    }
}