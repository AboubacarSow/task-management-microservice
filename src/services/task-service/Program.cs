using Microsoft.AspNetCore.Server.Kestrel.Core;
<<<<<<< HEAD
using shared.messaging.Interceptors;
=======
using shared.Configurations;
using shared.Extensions;
using shared.messaging.Interceptors;
using task_service.Tasks.Grpc.Client;
>>>>>>> 05b451b (new_update)


var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5003, o =>
    {
        o.Protocols = HttpProtocols.Http1;
    });

    options.ListenAnyIP(5006, o =>
    {
        o.Protocols = HttpProtocols.Http2;
    });
});
<<<<<<< HEAD
=======
MongoConfiguration.Configure();
>>>>>>> 05b451b (new_update)

builder.Host.UseCustomSerilog("taskmanagement");

builder.Services.AddOpenApi();

builder.Services.AddAuthentication("Bearer")
        .AddJwtBearer("Bearer", options =>
        {
           options.Authority = builder.Configuration["IdentityServer:Authority"];
           options.RequireHttpsMetadata = false; 
           options.Audience = "task-service";
            options.RefreshOnIssuerKeyNotFound= true;

            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = false, 
            };
            
        });
<<<<<<< HEAD
builder.Services
            .AddMassTransitWithAssembly(builder.Configuration,
=======
builder.Services.AddMassTransitWithAssembly(builder.Configuration,
>>>>>>> 05b451b (new_update)
            typeof(Program).Assembly);
//Grpc config
builder.Services.AddScoped<ServiceTokenInterceptor>();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
<<<<<<< HEAD
=======
builder.Services.AddSharedServices();

>>>>>>> 05b451b (new_update)

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
builder.Services.AddGrpcClient<ProjectInfo.ProjectInfoClient>(o =>
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

builder.Services.AddAuthorization();
//database config
builder.Services
       .Configure<DatabaseSettings>(builder.Configuration
       .GetSection(nameof(DatabaseSettings)));

builder.Services.AddDatabaseCollections();
builder.Services.ConfigureServices();



var app = builder.Build();
await app.CreateTaskIndexesAync();

app.MapCarter();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSerilogRequestLogging();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.UseMetrics(service:"task-service");

app.Run();

public partial class Program { }






