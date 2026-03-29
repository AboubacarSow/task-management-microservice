using Carter;
using Microsoft.IdentityModel.Tokens;
using project_service.Extensions;
using Prometheus;
using Serilog;
using shared.Behaviors;
using shared.Metrics;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseCustomSerilog("taskmanagement");

builder.Services.AddOpenApi();

builder.Services.AddAuthentication("Bearer")
        .AddJwtBearer("Bearer", options =>
        {
           options.Authority = "http://authentication-service:5000"; 
           options.Audience = "project-service";
      
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = false, 
           
            };
        });


builder.Services.AddAuthorization();
builder.Services
       .Configure<DatabaseSettings>(builder.Configuration
       .GetSection(nameof(DatabaseSettings)));
builder.Services.AddDatabaseCollections();
builder.Services.ConfigureServices();

var app = builder.Build();
app.UseMetrics();
await app.CreateTaskIndexesAync();
await app.CreateProjectIndexesAync();

app.MapCarter();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();


app.Run();

public partial class Program { }


