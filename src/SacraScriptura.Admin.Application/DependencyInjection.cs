using Microsoft.Extensions.DependencyInjection;
using SacraScriptura.Admin.Application.Bibles;
using SacraScriptura.Admin.Application.Books;
using SacraScriptura.Admin.Application.Divisions;

namespace SacraScriptura.Admin.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAdminApplication(this IServiceCollection services)
    {
        // Register Bible services
        services.AddScoped<BibleCreator>();
        services.AddScoped<BibleDeleter>();
        services.AddScoped<BibleFinder>();
        services.AddScoped<BibleSearcher>();
        services.AddScoped<BibleUpdater>();

        // Register Book services
        services.AddScoped<BookCreator>();
        services.AddScoped<BookDeleter>();
        services.AddScoped<BookFinder>();
        services.AddScoped<BookSearcher>();
        services.AddScoped<BookUpdater>();

        // Register Division services
        services.AddScoped<DivisionSearcher>();

        return services;
    }
}
