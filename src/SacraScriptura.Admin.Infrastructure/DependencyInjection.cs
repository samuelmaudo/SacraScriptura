using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SacraScriptura.Admin.Domain.Bibles;
using SacraScriptura.Admin.Domain.Books;
using SacraScriptura.Admin.Domain.Divisions;
using SacraScriptura.Admin.Infrastructure.Database;
using SacraScriptura.Admin.Infrastructure.Database.Repositories;

namespace SacraScriptura.Admin.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAdminInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Database Configuration
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
            )
        );

        // Register repositories
        services.AddScoped<IBibleRepository, BibleRepository>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IDivisionRepository, DivisionRepository>();

        return services;
    }
}
