namespace SurveyBasket.Api.Errors;

public class QuestionErrors
{
    public static readonly Error QuestionNotFound = new("Question.NotFound", "No question was found with given ID", StatusCodes.Status404NotFound);
    public static readonly Error DuplicatedQuestion = new("Question.DuplicatedQuestion", "A question with the same content already exists in the poll", StatusCodes.Status409Conflict);
}
