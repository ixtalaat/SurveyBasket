namespace SurveyBasket.Api.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials = new("User.InvalidCredentials", "Invalid email or password", StatusCodes.Status401Unauthorized);
    public static readonly Error InvalidToken = new("User.InvalidToken", "Invalid token", StatusCodes.Status401Unauthorized);
}
