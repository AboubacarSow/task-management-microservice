using project_service.Data.Utilities;
using project_service.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();


builder.Services
       .Configure<DatabaseSettings>(builder.Configuration.GetSection(nameof(DatabaseSettings)));
builder.Services.AddCollections();

var app = builder.Build();

await app.CreateTaskIndexesAync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();



app.Run();


