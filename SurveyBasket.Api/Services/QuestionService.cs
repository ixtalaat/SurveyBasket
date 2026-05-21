using SurveyBasket.Api.Contracts.Answers;

namespace SurveyBasket.Api.Services;

public class QuestionService(ApplicationDbContext context) : IQuestionService
{
    private readonly ApplicationDbContext _context = context;
    public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var pollIsExist = _context.Polls.Any(p => p.Id == pollId);
        if (!pollIsExist)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var questions = await _context.Questions
            .Where(q => q.PollId == pollId)
            .Include(x => x.Answers)
            .ProjectToType<QuestionResponse>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success(questions.Adapt<IEnumerable<QuestionResponse>>());
    }

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId, string userId, CancellationToken cancellationToken = default)
    {
        var hasVoted = await _context.Votes.AnyAsync(v => v.PollId == pollId && v.UserId == userId, cancellationToken);
        if (hasVoted)
            return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.DuplicatedVote);

        var pollIsExist = await _context.Polls.AnyAsync(p => p.Id == pollId && p.IsPublished && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);
        if (!pollIsExist)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var questions = await _context.Questions
            .Where(q => q.PollId == pollId && q.IsActive)
            .Include(x => x.Answers)
            .Select(q => new QuestionResponse(
                q.Id,
                q.Content,
                q.Answers.Where(a => a.IsActive).Select(a => new AnswerResponse(
                    a.Id,
                    a.Content
                )).ToList()
            ))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success(questions.Adapt<IEnumerable<QuestionResponse>>());
    }


    public async Task<Result<QuestionResponse>> GetAsync(int pollId, int id, CancellationToken cancellationToken = default)
    {
        var question = await _context.Questions
            .Where(q => q.PollId == pollId && q.Id == id)
            .Include(x => x.Answers)
            .ProjectToType<QuestionResponse>()
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (question == null)
            return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);

        return Result.Success(question);
    }
    public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        var pollIsExist = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);
        if (!pollIsExist)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

        var questionIsExist = await _context.Questions.AnyAsync(q => q.Content == request.Content && q.PollId == pollId, cancellationToken);
        if (questionIsExist)
            return Result.Failure<QuestionResponse>(QuestionErrors.DuplicatedQuestion);

        var question = request.Adapt<Question>();
        question.PollId = pollId;

        await _context.Questions.AddAsync(question, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(question.Adapt<QuestionResponse>());
    }

    public async Task<Result> UpdateAsync(int pollId, int id, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        var questionIsExist = await _context.Questions
            .AnyAsync(q => q.Content == request.Content 
                && q.Id != id
                && q.PollId == pollId, cancellationToken);

        if (questionIsExist)
            return Result.Failure<QuestionResponse>(QuestionErrors.DuplicatedQuestion);

        var question = await _context.Questions.Include(q => q.Answers).FirstOrDefaultAsync(q => q.Id == id && q.PollId == pollId, cancellationToken);

        if (question == null)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        question.Content = request.Content;

        var currentAnswers = question.Answers.Select(a => a.Content).ToList();
        var newAnswers = request.Answers.Except(currentAnswers).ToList();

        newAnswers.ForEach(answer =>
        {
            question.Answers.Add(new Answer { Content = answer });
        });

        question.Answers.ToList().ForEach(answer =>
        {
            answer.IsActive = request.Answers.Contains(answer.Content);
        });

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
    public async Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default)
    {
        var question = _context.Questions.FirstOrDefault(q => q.Id == id && q.PollId == pollId);
        if (question == null)
            return Result.Failure(QuestionErrors.QuestionNotFound);
        
        question.IsActive = !question.IsActive;
        
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
