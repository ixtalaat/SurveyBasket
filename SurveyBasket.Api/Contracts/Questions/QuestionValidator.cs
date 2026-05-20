namespace SurveyBasket.Api.Contracts.Questions;

public class QuestionValidator : AbstractValidator<QuestionRequest>
{
    public QuestionValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .Length(3, 1000);

        RuleFor(x => x.Answers)
            .NotNull();

        RuleFor(x => x.Answers)
            .Must(x => x.Count >= 2)
            .WithMessage("A question must have at least two answers.")
            .When(x => x.Answers != null);

        RuleFor(x => x.Answers)
            .Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("You cannot add duplicate answers for the same question.")
            .When(x => x.Answers != null);
    }
}
