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
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
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

    var category = new Category { Id = Guid.Parse("f410c901-998e-444d-ab01-1ae09e392cb4"), Name = "Programming" };
    context.Categories.Add(category);

    var user = new User
    {
        Id = Guid.Parse("af0d424e-8339-42f5-886d-197db2b6391d"),
        Email = "user@example.com",
        Password = "password123",
        Username = "Lxzer"
    };
    context.Users.Add(user);

    context.Courses.AddRange(
        new Course
        {
            Id = Guid.Parse("0ae968bc-f197-4662-925d-1c5abf404c83"),
            Title = "C# Basics",
            Description = "Learn the basics of C#",
            CategoryId = category.Id,
            UserId = user.Id,
        },
        new Course
        {
            Id = Guid.Parse("00735d47-22a0-41c9-a27c-1da7aa764034"),
            Title = "ASP.NET Core",
            Description = "Build web APIs",
            CategoryId = category.Id,
            UserId = user.Id,
        },
        new Course
        {
            Id = Guid.Parse("0cf3c3cf-7999-4556-a671-fed7604f7ad4"),
            Title = "Rust Basics",
            Description = "Learn the basics of Rust",
            CategoryId = category.Id,
            UserId = user.Id,
        }
    );

    context.SaveChanges();
}

app.Run();
