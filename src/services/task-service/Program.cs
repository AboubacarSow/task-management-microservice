using Microsoft.IdentityModel.Tokens;
using project_grpc_server;
using Serilog;
using shared.Behaviors;
using shared.Metrics;
using task_service.Data.Utilities;
using task_service.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseCustomSerilog("taskmanagement");

builder.Services.AddOpenApi();

builder.Services.AddAuthentication("Bearer")
        .AddJwtBearer("Bearer", options =>
        {
           options.Authority = builder.Configuration["IdentityServer:Authority"];
           options.RequireHttpsMetadata = false; 
           options.Audience = "task-service";
      
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

builder.Services.AddGrpcClient<ProjectInfo.ProjectInfoClient>(o =>
{
    o.Address = new Uri("http://localhost:5001"); // Docker service name
});

var app = builder.Build();
app.UseMetrics();
await app.CreateTaskIndexesAync();

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






