using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using SacraScriptura.Shared.Domain;
using SacraScriptura.Shared.Infrastructure.IA;
using SacraScriptura.Shared.Infrastructure.Options;

namespace SacraScriptura.Shared.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedInfrastructure(
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