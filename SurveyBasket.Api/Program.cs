using Hangfire;
using HangfireBasicAuthenticationFilter;
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

    app.UseHangfireDashboard("/jobs", new DashboardOptions
    {
        Authorization = [
            new HangfireCustomBasicAuthenticationFilter() {
                User = app.Configuration.GetValue<string>("HangfireSettings:Username"),
                Pass = app.Configuration.GetValue<string>("HangfireSettings:Password")
            }
        ],
        DashboardTitle = "Survey Basket - Hangfire Dashboard",
        //IsReadOnlyFunc = context => true
    });

    RecurringJob.AddOrUpdate<INotificationService>("SendNewPollsNotification", x => x.SendNewPollsNotification(), Cron.Daily);


    app.UseCors();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.UseExceptionHandler();

    app.Run();
}