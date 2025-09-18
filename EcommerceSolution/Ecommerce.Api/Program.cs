using AutoMapper;
using Ecommerce.Application.Mappings;
using Ecommerce.Application.services;
using Ecommerce.Domain.entities;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Infrastructure.services.Auth;
using Ecommerce.Infrastructure.services.AuthServices;
using Ecommerce.Infrastructure.services.ProductServices;
using Ecommerce.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add Controllers
builder.Services.AddControllers();

// Swagger + JWT config
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Ecommerce API", Version = "v1" });

    // JWT bearer configuration for Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// AutoMapper configuration
builder.Services.AddAutoMapper(typeof(ProductMappingProfile), typeof(UserMappingProfile));

// EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

// Password hasher
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Jwt service
builder.Services.AddScoped<IJwtService, JwtService>();

// Auth service
builder.Services.AddScoped<IAuthService, AuthService>();

// Add this line to register the ProductService for IProductService
builder.Services.AddScoped<IProductService, ProductService>();

// File storage service (for images)
builder.Services.AddScoped<IFileStorageService>(sp =>
    new FileStorageService(
        Path.Combine(sp.GetRequiredService<IWebHostEnvironment>().WebRootPath ?? "wwwroot")
    )
);

// Authentication
var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]);
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
        ValidateIssuerSigningKey = true,
        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Add CORS policy for Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Angular dev server
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ecommerce API v1");
        c.RoutePrefix = string.Empty; // optional → makes Swagger open at https://localhost:7010/
    });
}


app.UseStaticFiles(); // serve wwwroot/images

app.UseRouting();
app.UseAuthentication();



app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 401)
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"message\": \"Please login first.\"}");
    }
});
app.UseAuthorization();
app.UseCors("AllowAngular");

app.MapControllers();

// Seed a default admin 
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.Users.Any(u => u.Role == "Admin"))
    {
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        var admin = new User
        {
            UserName = "admin",Email = "admin@local",Role = "Admin",PasswordHash = hasher.HashPassword(null, "Admin@123")
        };

        db.Users.Add(admin);
        db.SaveChanges();
    }
}

app.Run();


public partial class Program { }
