using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using SurveyBasket.Api.Helpers;

namespace SurveyBasket.Api.Services;

public class NotificationService(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    IEmailSender emailSender,
    IHttpContextAccessor httpContextAccessor) : INotificationService
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task SendNewPollsNotification(int? pollId = null)
    {
        IEnumerable<Poll> polls = [];


        if (pollId.HasValue)
        {
            var poll = await _context.Polls
                .Where(x => x.Id == pollId && x.IsPublished)
                .SingleOrDefaultAsync();
            polls = [poll!];
        }
        else
        {
            polls = await _context.Polls
                .Where(x => x.IsPublished && x.StartsAt == DateOnly.FromDateTime(DateTime.Today))
                .AsNoTracking()
                .ToListAsync();
        }

        //TODO: Select memeber only
        var users = await _userManager.Users.ToListAsync();

        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
        
        foreach (var poll in polls)
        {
            foreach (var user in users)
            {
                var emailBody = EmailBodyBuilder.GenerateEmailBody("PollNotification", new Dictionary<string, string>
                    {
                        { "{{name}}", user.FirstName },
                        { "{{pollTill}}", poll.Title },
                        { "{{endDate}}", poll.EndsAt.ToString() },
                        { "{{url}}", $"{origin}/api/polls/{poll.Id}/vote" }
                    });

                await _emailSender.SendEmailAsync(user.Email!, "New Polls Available", emailBody);
            }
        }


    }
}
