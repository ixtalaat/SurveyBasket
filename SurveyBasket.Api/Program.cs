using Serilog;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddApi(builder.Configuration);

    builder.Host.UseSerilog((context, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration)
    );
}

var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
    }

    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();

    app.UseCors();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.UseExceptionHandler();

    app.Run();
}