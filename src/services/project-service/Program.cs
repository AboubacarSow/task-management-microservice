using Microsoft.IdentityModel.Tokens;
using project_service.Extensions;
using project_service.Projects.Grpc.Server;
using Serilog;
using shared.Behaviors;
using shared.Metrics;
using task_grpc_server;
using shared.messaging.Extensions;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using shared.messaging.Interceptors;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000, o =>
    {
        o.Protocols = HttpProtocols.Http1;
    });

    options.ListenAnyIP(5005, o =>
    {
        o.Protocols = HttpProtocols.Http2;
    });
});
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

builder.Host.UseCustomSerilog("taskmanagement");

builder.Services.AddOpenApi();
builder.Services.AddGrpc();

builder.Services.AddAuthentication("Bearer")
        .AddJwtBearer("Bearer", options =>
        {
           options.Authority = builder.Configuration["IdentityServer:Authority"];
           options.RequireHttpsMetadata = false; 
           options.Audience = "project-service";
           options.RefreshOnIssuerKeyNotFound= true;

      
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = false, 
           
            };
        });
builder.Services
    .AddMassTransitWithAssembly(builder.Configuration,typeof(Program).Assembly);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ServiceTokenInterceptor>();
builder.Services.AddMemoryCache();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("project_read", policy =>
    {
        policy.RequireAssertion(context =>
            context.User.HasClaim(c =>
                c.Type == "scope" &&
                c.Value.Split(' ').Contains("project_read")));
    });
});
    
builder.Services
       .Configure<DatabaseSettings>(builder.Configuration
       .GetSection(nameof(DatabaseSettings)));
builder.Services.AddDatabaseCollections();
builder.Services.ConfigureServices();

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

builder.Services.AddGrpcClient<TaskInfo.TaskInfoClient>(o =>
{
    o.Address = new Uri(builder.Configuration["GrpcServer:Host"]!);
}).ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new SocketsHttpHandler
        {
            EnableMultipleHttp2Connections = true,
            KeepAlivePingDelay = TimeSpan.FromSeconds(60),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
        };
        return handler;
    }).AddInterceptor<ServiceTokenInterceptor>();

var app = builder.Build();

app.MapGrpcService<ProjectsGrpcService>();

await app.CreateProjectIndexesAync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.UseMetrics(service:"project-service");

app.MapCarter();

app.Run();

public partial class Program { }


