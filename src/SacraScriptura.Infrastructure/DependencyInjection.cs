using SacraScriptura.Domain.Bibles;
using SacraScriptura.Domain.Books;
using SacraScriptura.Infrastructure.Database;
using SacraScriptura.Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SacraScriptura.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<ApplicationDbContext>(
            options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
                )
        );

        // Register repositories
        services.AddScoped<IBibleRepository, BibleRepository>();
        services.AddScoped<IBookRepository, BookRepository>();

        return services;
    }
}