using MassTransit.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Infrastructure.Authentication
{
    public class AuthenticationHandler : IAuthenticationHandler
    {
        // For Creating and Handling json Web Tokens
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        private readonly JwtOptions _options;
        private readonly SecurityKey _issuerSigninKey;
        private readonly SigningCredentials _credentials;
        // Contains json object that represents cryptographic operations applied to JWT
        private readonly JwtHeader _jwtHeader;
        // Contains set of params used by the Handler when validating a token
        private readonly TokenValidationParameters _tokenValidationParameters;
        public AuthenticationHandler(IConfiguration configuration)
        {
            _options = new JwtOptions();
            configuration.GetSection("jwt").Bind(_options);
            _issuerSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
            _credentials = new SigningCredentials(_issuerSigninKey, SecurityAlgorithms.HmacSha256);
            _jwtHeader = new JwtHeader(_credentials);
            _tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidIssuer = _options.Issuer,
                IssuerSigningKey = _issuerSigninKey
            };
        }

        public JwtAuthToken Create(string userId)
        {
            var nowUtc = DateTime.UtcNow;
            var expires = nowUtc.AddMinutes(_options.ExpiryMinutes);
            var centuryBegin = new DateTime(1970, 1, 1).ToUniversalTime();
            var exp = (long)(new TimeSpan(expires.Ticks - centuryBegin.Ticks).TotalSeconds);
            var now = (long)(new TimeSpan(nowUtc.Ticks - centuryBegin.Ticks).TotalSeconds);
            var payload = new JwtPayload
            {
                {"sub", userId },
                // Issuer
                {"iss", _options.Issuer },
                // Issue date
                {"iat", now },
                // Expiry
                {"exp", exp },
                {"unique_name", userId }
            };
            var jwt = new JwtSecurityToken(_jwtHeader, payload);
            var token = _jwtSecurityTokenHandler.WriteToken(jwt);
            var JsonToken = new JwtAuthToken{ 
                Token = token,
                Expires = exp  
            };

            return JsonToken;
        }
    }
}
