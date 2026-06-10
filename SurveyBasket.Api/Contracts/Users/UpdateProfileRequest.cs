namespace SurveyBasket.Api.Contracts.Users;

public record UpdateProfileRequest(
    string UserName,
    string FirstName,
    string LastName
);
