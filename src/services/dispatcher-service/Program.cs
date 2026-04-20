using dispatcher_service.Middlewares;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Cache.CacheManager;
using Serilog;
using shared.Behaviors;
using shared.Metrics;

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseCustomSerilog("dispatcher");

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
                     .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", 
        optional: true, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration)
    .AddCacheManager(op =>
    {
        op.WithDictionaryHandle();
    });

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "http://authentication-service:5004";
        options.SaveToken = true;
        options.MetadataAddress = "http://authentication-service:5004/.well-known/openid-configuration";

        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidIssuer = "http://authentication-service:5004"
        };
    });

builder.Services.AddAuthorization();


builder.Services.AddOpenApi();

var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSerilogRequestLogging();
app.UseMiddleware<GlobalExceptionHandler>(); 
app.UseMiddleware<CorrelationIdMiddleware>();



app.UseAuthentication();
app.UseAuthorization();

app.UseMetrics(service:"dispatcher-service");
// Skip Ocelot for /metrics
app.UseWhen(context =>
 !context.Request.Path.StartsWithSegments("/metrics"), 
 app => app.UseOcelot()
 );

app.Run();


