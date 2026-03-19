using Carter;
using Microsoft.IdentityModel.Tokens;
using project_service.Data.Utilities;
using project_service.Extensions;
using Serilog;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("serilog.json");
builder.Host.UseSerilog((context, configuration) =>
    {
        //To configure minimally serilog, we need to provide these two parameters
        configuration.ReadFrom
                .Configuration(context.Configuration);

        // Only add Elasticsearch outside of Test environment
        if (!context.HostingEnvironment.IsEnvironment("Test"))
        {
            configuration.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(
                new Uri(context.Configuration["Elasticsearch:Uri"]!))
            {
                IndexFormat = "taskmanagement-logs-{0:yyyy-MM}",
                AutoRegisterTemplate = true,
                NumberOfReplicas = 1,
                NumberOfShards=2,
                BatchAction=ElasticOpType.Create,
                BatchPostingLimit=50
            });
        }
    });

builder.Services.AddOpenApi();

builder.Services.AddAuthentication("Bearer")
        .AddJwtBearer("Bearer", options =>
        {
           options.Authority = "https://localhost:5001"; 
      
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = false, // Validate 
           
            };
        });


builder.Services.AddAuthorization();
builder.Services
       .Configure<DatabaseSettings>(builder.Configuration
       .GetSection(nameof(DatabaseSettings)));
builder.Services.AddDatabaseCollections();
builder.Services.ConfigureServices();

var app = builder.Build();

await app.CreateTaskIndexesAync();
await app.CreateProjectIndexesAync();

app.MapCarter();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();


app.Run();

public partial class Program { }


