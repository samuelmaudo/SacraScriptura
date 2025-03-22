using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SacraScriptura.Web.Domain.Bibles;
using SacraScriptura.Web.Domain.Books;
using SacraScriptura.Web.Domain.Divisions;
using SacraScriptura.Web.Infrastructure.Database.Repositories;

namespace SacraScriptura.Web.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddWebInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Database Configuration
        var databaseConnection = configuration.GetConnectionString("DefaultConnection");

        // Register repositories
        services.AddScoped<IBibleRecordRepository>(_ => new BibleRecordRepository(databaseConnection));
        services.AddScoped<IBookRecordRepository>(_ => new BookRecordRepository(databaseConnection));
        services.AddScoped<IDivisionRecordRepository>(_ => new DivisionRecordRepository(databaseConnection));

        return services;
    }
}