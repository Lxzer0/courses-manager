using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using CoursesManager.Models;
using CoursesManager.Interfaces;
using CoursesManager.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<CoursesManagerContext>(options =>
    options.UseInMemoryDatabase("CoursesManagerContext"));

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var _jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException("JWT settings not configured");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CoursesManagerContext>();

    var category = new Category { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "Programming" };
    context.Categories.Add(category);

    var user = new User
    {
        Id = Guid.Parse("12345678-1234-1234-1234-123456789abc"),
        Email = "user@example.com",
        Password = "password123",
        Username = "Lxzer"
    };
    context.Users.Add(user);

    context.Courses.AddRange(
        new Course
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Title = "C# Basics",
            Description = "Learn the basics of C#",
            CategoryId = category.Id,
            UserId = user.Id,
        },
        new Course
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Title = "ASP.NET Core",
            Description = "Build web APIs",
            CategoryId = category.Id,
            UserId = user.Id,
        },
        new Course
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Title = "Rust Basics",
            Description = "Learn the basics of Rust",
            CategoryId = category.Id,
            UserId = user.Id,
        }
    );

    context.SaveChanges();
}

app.Run();
