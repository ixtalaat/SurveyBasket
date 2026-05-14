namespace SurveyBasket.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var authResponse = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);
        if (authResponse is null)
            return BadRequest("Invalid email or password.");
        return Ok(authResponse);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var authResponse = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
        if (authResponse is null)
            return BadRequest("Invalid token or refresh token.");
        return Ok(authResponse);
    }

    [HttpPost("revoke-refresh-token")]
    public async Task<IActionResult> Revoke([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
        if (!result)
            return BadRequest("Operation failed.");
        return Ok();
    }
}
       