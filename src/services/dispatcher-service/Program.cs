using dispatcher_service.Middlewares;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;
using shared.Behaviors;
using shared.Metrics;

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseCustomSerilog("dispatcher");

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
                     .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", 
        optional: true, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["IdentityServer:Authority"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };                        
    });

builder.Services.AddAuthorization();


builder.Services.AddOpenApi();

var app = builder.Build();


app.UseMetrics();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSerilogRequestLogging();
app.UseMiddleware<GlobalExceptionHandler>(); 
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
await app.UseOcelot();

app.Run();


