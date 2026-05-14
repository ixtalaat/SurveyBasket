using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace SurveyBasket.Api.Services;

public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;

    private readonly int _refreshTokenExpirationDays = 14;
    public async Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return null;

        // check password
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
        if (!isPasswordValid)
            return null;

        // generate jwt token
        var (token, expiresIn) = _jwtProvider.GenerateToken(user);

        // generate refresh token
        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await _userManager.UpdateAsync(user);

        return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn, refreshToken, refreshTokenExpiration);
    }

    public async Task<AuthResponse?> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);
        if (userId is null)
            return null;

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return null;

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(rt => rt.Token == refreshToken && rt.IsActive);
        if (userRefreshToken is null)
            return null;

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        var (newToken, expiresIn) = _jwtProvider.GenerateToken(user);

        var newRefreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await _userManager.UpdateAsync(user);

        return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, newToken, expiresIn, newRefreshToken, refreshTokenExpiration);

    }
    private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    public async Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);
        if (userId is null)
            return false;

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return false;

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(rt => rt.Token == refreshToken && rt.IsActive);
        if (userRefreshToken is null)
            return false;

        userRefreshToken.RevokedOn = DateTime.UtcNow;
        
        await _userManager.UpdateAsync(user);

        return true;
    }
}
