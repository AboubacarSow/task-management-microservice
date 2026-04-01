using Microsoft.IdentityModel.Tokens;
using project_service.Extensions;
using project_service.Projects.Grpc.Server;
using Serilog;
using shared.Behaviors;
using shared.Interceptors;
using shared.Metrics;
using task_grpc_server;
using project_service;
using shared.messaging.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseCustomSerilog("taskmanagement");

builder.Services.AddOpenApi();
builder.Services.AddGrpc();

builder.Services.AddAuthentication("Bearer")
        .AddJwtBearer("Bearer", options =>
        {
           options.Authority = builder.Configuration["IdentityServer:Authority"];
           options.RequireHttpsMetadata = false; 
           options.Audience = "project-service";
      
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = false, 
           
            };
        });
builder.Services
    .AddMassTransitWitAssembly(builder.Configuration,typeof(Program).Assembly);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthenticationInterceptor>();
builder.Services.AddMemoryCache();
builder.Services.AddAuthorization(options =>
{
     options.AddPolicy("project_read", policy =>
     {
          policy.RequireClaim("scope", "project-service");
     });
    
});
    
builder.Services
       .Configure<DatabaseSettings>(builder.Configuration
       .GetSection(nameof(DatabaseSettings)));
builder.Services.AddDatabaseCollections();
builder.Services.ConfigureServices();

builder.Services.AddGrpcClient<TaskInfo.TaskInfoClient>(o =>
{
    o.Address = new Uri(builder.Configuration["GrpcServer:Host"]!);
}).AddInterceptor<AuthenticationInterceptor>();

var app = builder.Build();
app.UseMetrics();

app.MapGrpcService<ProjectsGrpcService>();

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


