using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Employee.Infrastructure.Data;
using Employee.Infrastructure.Repositories;
using Employee.Application.Interfaces;
using Employee.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<EmployeeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EmployeeDb") ?? 
        "Server=.;Database=EmployeeDataBase;Trusted_Connection=True;TrustServerCertificate=True;"));

// DI
builder.Services.AddScoped<EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

// Controllers & API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger configuration
var swaggerConfig = builder.Configuration.GetSection("Swagger");
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = swaggerConfig["Title"] ?? "Employee Service API",
        Description = swaggerConfig["Description"] ?? "Microservice for managing employees",
        Version = swaggerConfig["Version"] ?? "v1",
        Contact = new OpenApiContact
        {
            Name = swaggerConfig["Contact:Name"] ?? "Developer",
            Email = swaggerConfig["Contact:Email"] ?? "dev@example.com"
        }
    });

    // Add XML documentation comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (System.IO.File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// HTTP pipeline
app.UseHttpsRedirection();

// Swagger UI in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee Service API v1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}

app.UseAuthorization();
app.MapControllers();

app.Run();
