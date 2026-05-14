namespace SurveyBasket.Api.Persistence.EntitiesConfigruations;

public class UserConfigruation : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder
            .OwnsMany(u => u.RefreshTokens)
            .ToTable("RefreshTokens")
            .WithOwner()
            .HasForeignKey("UserId");
            

        builder.Property(p => p.FirstName)
            .HasMaxLength(100);

        builder.Property(p => p.LastName)
            .HasMaxLength(100);   
    }
}
