namespace SurveyBasket.Api.Persistence.EntitiesConfigruations;

public class VoteAnswerConfigruation : IEntityTypeConfiguration<VoteAnswer>
{
    public void Configure(EntityTypeBuilder<VoteAnswer> builder)
    {

        builder.HasIndex(x => new { x.VoteId, x.QuestionId })
            .IsUnique();
    }
}