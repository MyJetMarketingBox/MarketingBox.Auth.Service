using System;
using System.Threading.Tasks;
using MarketingBox.Auth.Service.Client.Interfaces;
using MarketingBox.Auth.Service.Domain.Models;
using MarketingBox.Auth.Service.Grpc;
using MarketingBox.Auth.Service.MyNoSql.Users;
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

    public async Task<User> GetUser(string tenantId, long userId)
    {
        try
        {
            _logger.LogInformation("Getting user from nosql server.");
            var noSqlResult = _noSqlReader.Get(tenantId, userId.ToString());
            if (noSqlResult != null)
            {
                return noSqlResult.User;
            }

            _logger.LogInformation("Getting user from grpc service.");
            var result = await _userService.GetAsync(new()
            {
                TenantId = tenantId,
                ExternalUserId = userId.ToString()
            });

            var user = result.Process();
            
            
            return user;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured while getting user.");
            throw;
        }
    }
}