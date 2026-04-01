using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

// Authentication/Authorization
var jwtSection = builder.Configuration.GetSection("Jwt");
var issuer = jwtSection["Issuer"];
var audience = jwtSection["Audience"];
var key = jwtSection["Key"];

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = signingKey
    };
});

builder.Services.AddAuthorization();

// Controllers & API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger configuration with JWT support
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

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (System.IO.File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT Bearer token in the format 'Bearer {token}'"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// HTTP pipeline
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee Service API v1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}

app.MapControllers();

app.Run();
