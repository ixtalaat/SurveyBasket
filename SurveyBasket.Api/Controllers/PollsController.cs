namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService pollsService) : ControllerBase
{
    private readonly IPollService _pollService = pollsService;

    [HttpGet]
    public IActionResult GetAll()
    {
        var polls = _pollService.GetAll();
        var response = polls.Adapt<IEnumerable<PollResponse>>();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult Get([FromRoute] int id)
    {
        var poll = _pollService.Get(id); 
        if (poll == null)
               return NotFound();

        var response = poll.Adapt<PollResponse>();

        return Ok(response);
    }

    [HttpPost]
    public IActionResult Add([FromBody] CreatePollRequest request)
    {
        var createdPoll = _pollService.Add(request.Adapt<Poll>());
        var response = createdPoll.Adapt<PollResponse>();
        return CreatedAtAction(nameof(Get), new { id = createdPoll.Id }, response);
    }

    [HttpPut("{id}")]
    public IActionResult Update([FromRoute] int id, [FromBody] CreatePollRequest request)
    {
        var isUpdated = _pollService.Update(id, request.Adapt<Poll>());
        if (!isUpdated)
            return NotFound();
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] int id)
    {
        var isDeleted = _pollService.Delete(id);
        if (!isDeleted)
            return NotFound();
        
        return NoContent();
    }
}