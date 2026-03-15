//using FastEndpoints;

//var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddFastEndpoints();

//var app = builder.Build();

//app.UseFastEndpoints();

//app.Run();
//using FastEndpoints;
//using FastEndpoints.Swagger;

//var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddFastEndpoints();
//builder.Services.SwaggerDocument();

//var app = builder.Build();

//app.UseFastEndpoints();
//app.UseSwaggerGen();

//app.Run();

using FastEndpoints;
using FastEndpoints.Swagger;
using TaskManager.Infrastructure.Extensions;
using TaskManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();
builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("Default")!);

var app = builder.Build();

// Auto-migrate on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseFastEndpoints();
app.UseSwaggerGen();

app.Run();

public partial class Program { }