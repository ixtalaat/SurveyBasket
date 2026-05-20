namespace SurveyBasket.Api.Errors;

public class QuestionErrors
{
    public static Error QuestionNotFound = new("Question.NotFound", "No question was found with given ID");
    public static Error QuestionAlreadyExists = new("Question.AlreadyExists", "A question with the same content already exists in the poll");
}
