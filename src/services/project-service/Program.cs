using Carter;
using project_service.Data.Utilities;
using project_service.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();


builder.Services
       .Configure<DatabaseSettings>(builder.Configuration.GetSection(nameof(DatabaseSettings)));
builder.Services.AddCollections();
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



app.Run();

public partial class Program { }


