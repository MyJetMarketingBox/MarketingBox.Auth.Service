using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MarketingBox.Auth.Service.Grpc;
using MarketingBox.Auth.Service.Grpc.Models;
using MarketingBox.Auth.Service.Repositories.Interfaces;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.Grpc;
using MarketingBox.Sdk.Crypto;
using Microsoft.IdentityModel.Tokens;

namespace MarketingBox.Auth.Service.Services
{
    public class TokensService : ITokensService
    {
        private const string UserIdClaim = "user-id";
        private const string UserNameClaim = "user-name";
        private const string TenantIdClaim = "tenant-id";


        private readonly IUserRepository _userRepository;
        private readonly ICryptoService _cryptoService;
        private readonly string _tokenSecret;
        private readonly string _mainAudience;
        private readonly TimeSpan _ttl;

        public TokensService(IUserRepository userRepository,
            ICryptoService cryptoService)
        {
            _userRepository = userRepository;
            _cryptoService = cryptoService;
            TimeSpan.TryParse(Program.Settings.JwtTtl, out var ttl);
            _ttl = ttl;
            _tokenSecret = Program.Settings.JwtSecret;
            _mainAudience = Program.Settings.JwtAudience;
        }

        public async Task<Response<TokenInfo>> LoginAsync(TokenRequest request)
        {
            try
            {
                var isEmail = MailAddress.TryCreate(request.Login, out var _);

                string passHash = null;
                string userSalt = null;
                string userName = null;

                var (users, _) = await _userRepository.SearchAsync(new SearchUserRequest()
                {
                    Username = !isEmail ? request.Login : null,
                    Email = isEmail ? request.Login : null,
                    TenantId = request.TenantId
                });

                if (!users.Any())
                {
                    throw new NotFoundException(NotFoundException.DefaultMessage);
                }

                if (users.Count > 1)
                {
                    throw new InvalidOperationException(
                        "There can not be more than 1 user for tenant and login");
                }

                var user = users.First();

                passHash = user.PasswordHash;
                userSalt = user.Salt;
                userName = user.Username;

                if (!_cryptoService.VerifyHash(userSalt, request.Password, passHash))
                {
                    throw new BadRequestException("Password is wrong");
                }

                var expAt = DateTime.UtcNow + _ttl;
                var token = new TokenInfo()
                {
                    Token = Create(request.TenantId, userName, expAt, user.ExternalUserId),
                    ExpiresAt = expAt
                };
                return new Response<TokenInfo>()
                {
                    Data = token,
                    Status = ResponseStatus.Ok
                };
            }
            catch (Exception e)
            {
                return e.FailedResponse<TokenInfo>();
            }
        }

        private string Create(string tenantId, string username, DateTime expirationDate, string userId)
        {
            var properties = new Dictionary<string, string>
            {
                {UserNameClaim, username},
                {TenantIdClaim, tenantId},
                {UserIdClaim, userId},
            };

            var audiences = new List<string>()
            {
                _mainAudience
            };

            return Create(expirationDate, properties, audiences);
        }

        private string Create(DateTime expirationDate, IReadOnlyDictionary<string, string> properties,
            IEnumerable<string> audiences)
        {
            var key = Encoding.ASCII.GetBytes(_tokenSecret);

            var claims = audiences
                .Select(audience => new Claim(JwtRegisteredClaimNames.Aud, audience))
                .ToList();

            claims.AddRange(properties.Select(property => new Claim(property.Key, property.Value)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expirationDate,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}