using Microsoft.Extensions.DependencyInjection;
using SacraScriptura.Application.Bibles;
using SacraScriptura.Application.Books;
using SacraScriptura.Application.Divisions;

namespace SacraScriptura.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register services
        services.AddScoped<BibleService>();
        services.AddScoped<BookService>();
        services.AddScoped<DivisionService>();

        return services;
    }
}