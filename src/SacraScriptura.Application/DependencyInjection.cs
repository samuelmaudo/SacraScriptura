using Microsoft.Extensions.DependencyInjection;
using SacraScriptura.Application.Bibles;

namespace SacraScriptura.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register services
        services.AddScoped<IBibleService, BibleService>();

        return services;
    }
}