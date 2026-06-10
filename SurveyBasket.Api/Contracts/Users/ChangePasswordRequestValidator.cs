using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Contracts.Users;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .Matches(RegexPatterns.Password)
            .WithMessage("Password must contain at least 8 characters and should contain uppercase letter,  lowercase letter, digit, and special character.")
            .NotEqual(x => x.CurrentPassword)
            .WithMessage("New password cannot be the same as the current password.");
    }
}
