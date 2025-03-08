using Microsoft.Extensions.DependencyInjection;
using SacraScriptura.Application.Bibles;
using SacraScriptura.Application.Books;

namespace SacraScriptura.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register services
        services.AddScoped<BibleService>();
        services.AddScoped<BookService>();

        return services;
    }
}