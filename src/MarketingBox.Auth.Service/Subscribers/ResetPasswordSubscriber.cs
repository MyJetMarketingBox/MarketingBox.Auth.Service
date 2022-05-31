using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using DotNetCoreDecorators;
using MarketingBox.Auth.Service.Engines;
using MarketingBox.PasswordApi.Domain.Models;
using Microsoft.Extensions.Logging;

namespace MarketingBox.Auth.Service.Subscribers
{
    public class ResetPasswordSubscriber : IStartable
    {
        private readonly ILogger<ResetPasswordSubscriber> _logger;
        private readonly ResetPasswordEngine _resetPasswordEngine;

        public ResetPasswordSubscriber(ILogger<ResetPasswordSubscriber> logger,
            ISubscriber<IReadOnlyList<PasswordResetMessage>> subscriber,
            ResetPasswordEngine resetPasswordEngine)
        {
            _logger = logger;
            _resetPasswordEngine = resetPasswordEngine;

            subscriber.Subscribe(HandleEvents);
        }

        private async ValueTask HandleEvents(IReadOnlyList<PasswordResetMessage> events)
        {
            try
            {
                _logger.LogInformation($"{nameof(ResetPasswordSubscriber)} receive {events.Count} events.");

                foreach (var elem in events)
                {
                    await _resetPasswordEngine.HandleAffiliate(elem);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public void Start()
        {
        }
    }
}