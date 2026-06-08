using Hangfire;
using SurveyBasket.Api.Contracts.Polls;

namespace SurveyBasket.Api.Services;

public class PollService(ApplicationDbContext context, INotificationService notificationService) : IPollService
{
    private readonly ApplicationDbContext _context = context;
    private readonly INotificationService _notificationService = notificationService;

    public async Task<IEnumerable<PollResponse>> GetAllAsync(CancellationToken cancellationToken = default) => await _context.Polls
        .AsNoTracking()
        .ProjectToType<PollResponse>()
        .ToListAsync(cancellationToken);

    public async Task<IEnumerable<PollResponse>> GetCurrentAsync(CancellationToken cancellationToken = default) => await _context.Polls
        .AsNoTracking()
        .Where(p => p.IsPublished && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow))
        .ProjectToType<PollResponse>()
        .ToListAsync(cancellationToken);

    public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.FindAsync(id, cancellationToken);
        return poll is null
            ? Result.Failure<PollResponse>(PollErrors.PollNotFound)
            : Result.Success(poll.Adapt<PollResponse>());
    }

    public async Task<Result<PollResponse>> AddAsync(PollRequest pollRequest, CancellationToken cancellationToken = default)
    {
        var isExistingTitle = await _context.Polls.AnyAsync(poll => poll.Title == pollRequest.Title, cancellationToken);
        
        if (isExistingTitle)
            return Result.Failure<PollResponse>(PollErrors.DuplicatedPollTitle);

        var poll = pollRequest.Adapt<Poll>();
        await _context.Polls.AddAsync(poll, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success(poll.Adapt<PollResponse>());
    }

    public async Task<Result> UpdateAsync(int id, PollRequest pollRequest, CancellationToken cancellationToken = default)
    {
        var isExistingTitle = await _context.Polls.AnyAsync(p => p.Title == pollRequest.Title && p.Id != id, cancellationToken);

        if (isExistingTitle)
            return Result.Failure<PollResponse>(PollErrors.DuplicatedPollTitle);


        var currentPoll = await _context.Polls.FindAsync(id, cancellationToken);
        if (currentPoll is null)
            return Result.Failure(PollErrors.PollNotFound);

        currentPoll.Title = pollRequest.Title;
        currentPoll.Summary = pollRequest.Summary;
        currentPoll.StartsAt = pollRequest.StartsAt;
        currentPoll.EndsAt = pollRequest.EndsAt;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var currentPoll = await _context.Polls.FindAsync(id, cancellationToken);
        if (currentPoll is null)
            return Result.Failure(PollErrors.PollNotFound);

        _context.Polls.Remove(currentPoll);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var currentPoll = await _context.Polls.FindAsync(id, cancellationToken);
        if (currentPoll is null)
            return Result.Failure(PollErrors.PollNotFound);

        currentPoll.IsPublished = !currentPoll.IsPublished;

        await _context.SaveChangesAsync(cancellationToken);

        if (currentPoll.IsPublished && currentPoll.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
            BackgroundJob.Enqueue(() => _notificationService.SendNewPollsNotification (currentPoll.Id));

        return Result.Success();
    }
}