using Microsoft.Extensions.DependencyInjection;
using SacraScriptura.Domain.Divisions;

namespace SacraScriptura.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        // Register services
        services.AddScoped<DivisionService>();

        return services;
    }
}