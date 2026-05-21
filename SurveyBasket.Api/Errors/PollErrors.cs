namespace SurveyBasket.Api.Errors;

public class PollErrors
{
    public static readonly Error PollNotFound = new("Poll.NotFound", "No poll was found with given ID", StatusCodes.Status404NotFound);
    public static readonly Error DuplicatedPollTitle = new("Poll.DuplicatedTitle", "A poll with the same title already exists", StatusCodes.Status409Conflict);
}
