namespace SurveyBasket.Api.Controllers;

[Route("api/polls/{pollId}/vote")]
[ApiController]
[Authorize]
public class VotesController(IVoteService voteService, IQuestionService questionService) : ControllerBase
{
    private readonly IVoteService _voteService = voteService;
    private readonly IQuestionService _questionService = questionService;

    [HttpGet]
    public async Task<IActionResult> Start([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await _questionService.GetAvailableAsync(pollId, userId!, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost]
    public async Task<IActionResult> Vote([FromRoute] int pollId, [FromBody] VoteRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await _voteService.AddAsync(pollId, userId!, request, cancellationToken);

        return result.IsSuccess ? Created() : result.ToProblem();
    }
}
