using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using SacraScriptura.Domain.Bibles;
using SacraScriptura.Domain.Books;
using SacraScriptura.Domain.Common;
using SacraScriptura.Domain.Divisions;
using SacraScriptura.Infrastructure.AI;
using SacraScriptura.Infrastructure.Database;
using SacraScriptura.Infrastructure.Database.Repositories;
using SacraScriptura.Infrastructure.Options;

namespace SacraScriptura.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Database Configuration
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
        services.AddScoped<IDivisionRepository, DivisionRepository>();

        // Configure AI services
        services.AddOllama(configuration);

        return services;
    }

    private static IServiceCollection AddOllama(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<OllamaOptions>(
            configuration.GetSection(OllamaOptions.SectionName)
        );
        var serviceProvider = services.BuildServiceProvider();
        var ollamaOptions = serviceProvider.GetRequiredService<IOptions<OllamaOptions>>().Value;

#pragma warning disable SKEXP0070
        services.AddOllamaTextEmbeddingGeneration(
            ollamaOptions.EmbeddingsModel,
            new HttpClient
            {
                BaseAddress = new Uri(ollamaOptions.BaseUrl),
                Timeout = TimeSpan.FromSeconds(ollamaOptions.TimeoutSeconds)
            }
        );
#pragma warning restore SKEXP0070

        services.AddScoped<IEmbeddingsGenerator, SemanticKernelEmbeddingsGenerator>();

        return services;
    }
}