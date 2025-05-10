using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Lalasia_store.Shared.Config;
using Lalasia_store.Shared.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Lalasia_store.Services;

public class AuthTokensService : IAuthTokensService
{
    private readonly IOptions<JwtSettings> _jwtSettings;

    public AuthTokensService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }
    
    public string GenerateAccessToken(Guid userId)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        // var key = _jwtSettings.Value.SecretKey;
        // var testkey = Encoding.ASCII.GetBytes(key);
        //
        // var tokeDescriptor = new SecurityTokenDescriptor
        // {
        //     Expires = DateTime.Now.AddHours(1),
        //     Issuer = "mospolytech",
        //     Audience = "access",
        //     SigningCredentials =
        //         new SigningCredentials(
        //             new SymmetricSecurityKey(testkey),
        //             SecurityAlgorithms.HmacSha256Signature
        //         )
        // };
        //
        // var tokenHandler = new JwtSecurityTokenHandler();
        // var token = tokenHandler.CreateToken(tokeDescriptor);
        //
        // var jwtToken = tokenHandler.WriteToken(token);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken(Guid userId)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: DateTime.Now.AddMonths(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}