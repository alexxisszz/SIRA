using Microsoft.Extensions.DependencyInjection;
using SistemaRefuerzo.Application.Evaluaciones;
using SistemaRefuerzo.Application.Recomendaciones;

namespace SistemaRefuerzo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddScoped<GeneradorDeRecomendacion>();
        services.AddScoped<ProgresoTemaService>();

        return services;
    }
}