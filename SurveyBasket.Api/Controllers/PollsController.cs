namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService pollsService) : ControllerBase
{
    private readonly IPollService _pollService = pollsService;

    [HttpGet]
    public ActionResult<IEnumerable<Poll>> GetAll ()
    {
        var _polls = _pollService.GetAll();
        return Ok(_polls);
    }

    [HttpGet("{id}")]
    public ActionResult<Poll> Get(int id)
    {
        var poll = _pollService.Get(id); 
        if (poll == null)
               return NotFound();
        
        return Ok(poll);
    }

    [HttpPost]
    public ActionResult<Poll> Add(Poll request)
    {
        var createdPoll = _pollService.Add(request);
        return CreatedAtAction(nameof(Get), new { id = createdPoll.Id }, createdPoll);
    }

    [HttpPut("{id}")]
    public ActionResult<Poll> Update(int id, Poll request)
    {
        var isUpdated = _pollService.Update(id, request);
        if (!isUpdated)
            return NotFound();
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var isDeleted = _pollService.Delete(id);
        if (!isDeleted)
            return NotFound();
        
        return NoContent();
    }
}