using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SistemaRefuerzo.Api.Middleware;
using SistemaRefuerzo.Application;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Infrastructure;
using SistemaRefuerzo.Infrastructure.Persistence;
using SistemaRefuerzo.Infrastructure.Persistence.Seed;
using SistemaRefuerzo.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

const string PoliticaCorsFrontend = "FrontendAngular";

var puerto = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(puerto))
    builder.WebHost.UseUrls($"http://0.0.0.0:{puerto}");

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy(PoliticaCorsFrontend, policy =>
        policy.WithOrigins(builder.Configuration.GetSection("FrontendUrls").Get<string[]>() ?? [])
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var jwtSettings = builder.Configuration.GetSection(JwtSettings.SeccionConfiguracion).Get<JwtSettings>()
    ?? throw new InvalidOperationException("La sección 'Jwt' no está configurada.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Emisor,
            ValidAudience = jwtSettings.Audiencia,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.ClaveSecreta)),
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    await DbInitializer.SeedAsync(dbContext, passwordHasher);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors(PoliticaCorsFrontend);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
