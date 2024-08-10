using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using AuthService.Model;
using AuthService.Dtos;
using AuthService.Providers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddScoped<TokenProvider>();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/login", (LoginRequest request, TokenProvider tokenProvider) =>
{
    //Just for testing. Member must authenticate with another identity store
    var member = new Member(1, "john.doe@azonworks.fun", "jhondoe", "123456");

    if (request.MemberName == member.Name && request.Password == member.Password)
    {
        var token = tokenProvider.GenerateToken(member);
        return Results.Ok(new { Token = token });
    }

    return Results.Unauthorized();
});

app.Run();