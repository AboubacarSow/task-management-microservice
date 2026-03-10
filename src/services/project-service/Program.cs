using project_service.Data.Utilities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();


builder.Services
       .Configure<DatabaseSettings>(builder.Configuration.GetSection(nameof(DatabaseSettings)));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();



app.Run();


