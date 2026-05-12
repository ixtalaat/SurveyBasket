namespace SurveyBasket.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

        services
            .AddOpenApi()
            .AddMapsterConf()
            .AddFluentValidationConf()
            .AddDbContextConf(configuration);


        services.AddScoped<IPollService, PollService>();
        
        return services;
    }

    private static IServiceCollection AddDbContextConf(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        return services;
    }

    private static IServiceCollection AddMapsterConf(this IServiceCollection services)
    {
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton<IMapper>(new Mapper(mappingConfig));
        return services;
    }

    private static IServiceCollection AddFluentValidationConf(this IServiceCollection services)
    {
        services
            .AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }
}
