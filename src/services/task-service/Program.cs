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
builder.Services
            .AddMassTransitWithAssembly(builder.Configuration,
            typeof(Program).Assembly);
//Grpc config
builder.Services.AddScoped<AuthenticationInterceptor>();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddGrpcClient<ProjectInfo.ProjectInfoClient>(o =>
{
    o.Address = new Uri(builder.Configuration["GrpcServer:Host"]!);
}).AddInterceptor<AuthenticationInterceptor>();

builder.Services.AddAuthorization();
//database config
builder.Services
       .Configure<DatabaseSettings>(builder.Configuration
       .GetSection(nameof(DatabaseSettings)));

builder.Services.AddDatabaseCollections();
builder.Services.ConfigureServices();



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






