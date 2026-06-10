using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Contracts.Auth;

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
        RuleFor(x => x.Code)
            .NotEmpty();
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .Matches(RegexPatterns.Password)
            .WithMessage("Password must contain at least 8 characters and should contain uppercase letter,  lowercase letter, digit, and special character.");
    }
}
