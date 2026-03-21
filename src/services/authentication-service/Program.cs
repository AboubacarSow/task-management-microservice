using authentication_service;
using authentication_service.Services;
using authentication_service.Validators;
using Duende.IdentityServer.Licensing;
using Serilog;
using shared.Behaviors;
using System.Globalization;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    //builder.Configuration.AddJsonFile("serilog.json");
    builder.Host.UseCustomSerilog();
    // Program.cs
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddHttpClient<UserProfileService>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["UserService:BaseUrl"]!);
    });
    builder.Services.AddHttpClient<IdentityResourceOwnerPasswordValidator>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["UserService:BaseUrl"]!);
    });

    var app = builder
        .ConfigureServices()
        .ConfigurePipeline();

    if (app.Environment.IsDevelopment())
    {
        app.Lifetime.ApplicationStopping.Register(() =>
        {
            var usage = app.Services.GetRequiredService<LicenseUsageSummary>();
            Console.Write(Summary(usage));
        });
    }

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}

static string Summary(LicenseUsageSummary usage)
{
    var sb = new StringBuilder();
    sb.AppendLine("IdentityServer Usage Summary:");
    sb.AppendLine(CultureInfo.InvariantCulture, $"  License: {usage.LicenseEdition}");
    var features = usage.FeaturesUsed.Count > 0 ? string.Join(", ", usage.FeaturesUsed) : "None";
    sb.AppendLine(CultureInfo.InvariantCulture, $"  Business and Enterprise Edition Features Used: {features}");
    sb.AppendLine(CultureInfo.InvariantCulture, $"  {usage.ClientsUsed.Count} Client Id(s) Used");
    sb.AppendLine(CultureInfo.InvariantCulture, $"  {usage.IssuersUsed.Count} Issuer(s) Used");

    return sb.ToString();
}
