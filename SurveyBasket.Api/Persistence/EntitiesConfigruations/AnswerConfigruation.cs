namespace SurveyBasket.Api.Persistence.EntitiesConfigruations;

public class AnswerConfigruation : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {

        builder.HasIndex(a => new { a.Content, a.QuestionId })
            .IsUnique();

        builder.Property(a => a.Content)
            .HasMaxLength(1000);
    }
}