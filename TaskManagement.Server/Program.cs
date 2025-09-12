using Microsoft.OpenApi.Models;
using TaskManagement.Server.Common.Configuration;
using TaskManagement.Server.Common.Extensions;
using TaskManagement.Server.Endpoints;
using TaskManagement.Server.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

// Configure application settings
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = appSettings?.Swagger.Title ?? "TaskManagement.Server", 
        Version = appSettings?.Swagger.Version ?? "v1",
        Description = appSettings?.Swagger.Description ?? "Task Management API"
    });
});

// Register repositories and services
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();

// Enable Swagger only in Development
if (app.Environment.IsDevelopment())
{
    app.UseDataSetup();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManagement.Server");
    });
}

app.MapFallbackToFile("/index.html");

app.MapBoardEndpoints();
app.MapBoardTaskEndpoints();
app.MapTaskAttachmentEndpoints();
app.MapUserFavoriteEndpoints();
app.Run();