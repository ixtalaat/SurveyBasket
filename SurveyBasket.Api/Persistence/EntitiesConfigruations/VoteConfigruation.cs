namespace SurveyBasket.Api.Persistence.EntitiesConfigruations;

public class VoteConfigruation : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {

        builder.HasIndex(x => new { x.UserId, x.PollId })
            .IsUnique();
    }
}