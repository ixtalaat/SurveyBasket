namespace SurveyBasket.Api.Errors;

public class VoteErrors
{
    public static readonly Error InvalidQuestions = new("Vote.InvalidQuestions", "Invalid questions provided", StatusCodes.Status400BadRequest);
    public static readonly Error DuplicatedVote = new("Vote.DuplicatedVote", "This user has already voted for this poll", StatusCodes.Status409Conflict);
}
