using Microsoft.Extensions.DependencyInjection;
using SacraScriptura.Admin.Domain.Divisions;

namespace SacraScriptura.Admin.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddAdminDomain(this IServiceCollection services)
    {
        // Register services
        services.AddScoped<DivisionService>();

        return services;
    }
}
