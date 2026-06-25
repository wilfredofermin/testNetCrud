using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using testNet.API.Middleware;
using testNet.Application.Interfaces;
using testNet.Application.Services;
using testNet.Infrastructure;
using testNet.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "testNet API",
        Version = "v1",
        Description = "API RESTful para gestión de productos. Casos de uso: CRUD completo, control de inventario, consultas paginadas y filtrado por estado.",
        Contact = new OpenApiContact
        {
            Name = "testNet",
            Email = "dev@testnet.com"
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddInfrastructure();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
    await DbSeeder.SeedAsync(context);
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseCors();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "testNet API v1");
    options.DefaultModelsExpandDepth(-1);
});

app.MapGet("/redoc", async context =>
{
    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync("""
<!DOCTYPE html>
<html>
<head>
    <title>testNet API - Redoc</title>
    <meta charset="utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link href="https://fonts.googleapis.com/css?family=Montserrat:300,400,700|Roboto:300,400,700" rel="stylesheet">
</head>
<body>
    <div id="redoc-container"></div>
    <script src="https://cdn.redoc.ly/redoc/latest/bundles/redoc.standalone.js"></script>
    <script>
        Redoc.init('/swagger/v1/swagger.json', {
            scrollYOffset: 0,
            hideDownloadButton: false,
            expandResponses: '200'
        }, document.getElementById('redoc-container'));
    </script>
</body>
</html>
""");
});

app.MapPost("/api/seed", async (AppDbContext context) =>
{
    var count = await DbSeeder.ResetAndSeedAsync(context);
    return Results.Ok(new { message = $"Database reset and seeded with {count} products.", count });
})
.WithTags("Seed")
.WithOpenApi(operation =>
{
    operation.Summary = "Resetea y siembra la base de datos con productos de prueba";
    operation.Description = "Elimina todos los productos existentes y los reemplaza con 10 productos de ejemplo (LAP-001 a DIS-001). Útil para desarrollo y testing.";
    return operation;
});

app.MapControllers();

app.Run();

public partial class Program { }
