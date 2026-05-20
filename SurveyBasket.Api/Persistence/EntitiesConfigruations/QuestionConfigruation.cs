namespace SurveyBasket.Api.Persistence.EntitiesConfigruations;

public class QuestionConfigruation : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        
        builder.HasIndex(q => new { q.Content, q.PollId })
            .IsUnique();

        builder.Property(q => q.Content)
            .HasMaxLength(1000);
    }
}