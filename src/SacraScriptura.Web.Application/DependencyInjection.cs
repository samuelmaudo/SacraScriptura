using Microsoft.Extensions.DependencyInjection;
using SacraScriptura.Web.Application.Bibles;
using SacraScriptura.Web.Application.Books;
using SacraScriptura.Web.Application.Divisions;

namespace SacraScriptura.Web.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddWebApplication(this IServiceCollection services)
    {
        // Register Bible services
        services.AddScoped<BibleRecordFinder>();
        services.AddScoped<BibleRecordSearcher>();

        // Register Book services
        services.AddScoped<BookRecordFinder>();
        services.AddScoped<BookRecordSearcher>();

        // Register Division services
        services.AddScoped<DivisionRecordFinder>();
        services.AddScoped<DivisionRecordSearcher>();

        return services;
    }
}
