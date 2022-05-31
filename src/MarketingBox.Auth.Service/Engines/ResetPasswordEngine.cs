using System;
using System.Threading.Tasks;
using MarketingBox.Auth.Service.Grpc;
using MarketingBox.Auth.Service.Grpc.Models;
using MarketingBox.PasswordApi.Domain.Models;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Extensions;
using Microsoft.Extensions.Logging;

namespace MarketingBox.Auth.Service.Engines
{
    public class ResetPasswordEngine
    {
        private readonly ILogger<ResetPasswordEngine> _logger;
        private readonly IUserService _userService;

        public ResetPasswordEngine(ILogger<ResetPasswordEngine> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public async Task HandleAffiliate(PasswordResetMessage message)
        {
            try
            {
                if (!long.TryParse(message.UserId, out var id))
                {
                    throw new BadRequestException($"Can't parse user id: {message.UserId}");
                }

                var result = await _userService.ForceChangePasswordAsync(new ForceChangePasswordRequest()
                {
                    NewPassword = message.NewPassword,
                    TenantId = message.TenantId,
                    UserId = id,
                    ChangedByUserId = id
                });
                result.Process();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occured while processing reset password event {@Message}", message);
            }
        }
    }
}