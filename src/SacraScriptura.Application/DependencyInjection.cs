using Microsoft.Extensions.DependencyInjection;
using SacraScriptura.Application.Bibles;
using SacraScriptura.Application.Books;
using SacraScriptura.Application.Divisions;

namespace SacraScriptura.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
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
        
        // Register other services
        services.AddScoped<DivisionService>();

        return services;
    }
}