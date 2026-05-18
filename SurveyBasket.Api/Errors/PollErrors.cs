namespace SurveyBasket.Api.Errors;

public class PollErrors
{
    public static Error PollNotFound = new("Poll.NotFound", "No poll was found with given ID");
}
