using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SurveyBasket.Api.Authentication;

public class JwtProvider(IOptions<JwtOptions> jwtOptions) : IJwtProvider
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public (string token, int expiresIn) GenerateToken(ApplicationUser user)
    {
        var claims = new ClaimsIdentity(
                [
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

                ]);

        var key = Encoding.UTF8.GetBytes(_jwtOptions.Key);
        var symmetricSecurityKey = new SymmetricSecurityKey(key);
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);


        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims.Claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
            signingCredentials: signingCredentials
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), _jwtOptions.ExpiryMinutes * 60);
    }

    public string? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtOptions.Key);
        var symmetricSecurityKey = new SymmetricSecurityKey(key);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                IssuerSigningKey = symmetricSecurityKey,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
            return userId;
        }
        catch
        {
            return null;
        }
    }
}
