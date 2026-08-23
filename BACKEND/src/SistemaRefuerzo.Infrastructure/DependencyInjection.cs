using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Infrastructure.Persistence;
using SistemaRefuerzo.Infrastructure.Persistence.Repositories;
using SistemaRefuerzo.Infrastructure.Security;

namespace SistemaRefuerzo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres")));

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SeccionConfiguracion));

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IAlumnoRepository, AlumnoRepository>();
        services.AddScoped<IDocenteRepository, DocenteRepository>();
        services.AddScoped<ITemaRepository, TemaRepository>();
        services.AddScoped<IPreguntaRepository, PreguntaRepository>();
        services.AddScoped<IEvaluacionRepository, EvaluacionRepository>();
        services.AddScoped<IReglaRepository, ReglaRepository>();
        services.AddScoped<IResultadoRepository, ResultadoRepository>();
        services.AddScoped<IRecomendacionRepository, RecomendacionRepository>();
        services.AddScoped<IDocenteQueryRepository, DocenteQueryRepository>();
        services.AddScoped<IAdminQueryRepository, AdminQueryRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}