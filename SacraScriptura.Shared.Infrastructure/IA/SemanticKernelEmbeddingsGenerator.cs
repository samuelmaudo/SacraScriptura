using Microsoft.SemanticKernel.Embeddings;
using SacraScriptura.Shared.Domain;

namespace SacraScriptura.Shared.Infrastructure.IA;

/// <summary>
/// Implementation of the embeddings generator using Ollama with Semantic Kernel.
/// </summary>
public class SemanticKernelEmbeddingsGenerator(
#pragma warning disable SKEXP0001
    ITextEmbeddingGenerationService embeddingService
#pragma warning restore SKEXP0001
) : IEmbeddingsGenerator
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<float>> GenerateEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default
    )
    {
        var embedding = await embeddingService.GenerateEmbeddingAsync(
            text,
            cancellationToken: cancellationToken
        );
        return embedding.ToArray();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<IReadOnlyList<float>>> GenerateEmbeddingsAsync(
        IEnumerable<string> texts,
        CancellationToken cancellationToken = default
    )
    {
        var textsList = texts.ToList();

        var embeddings = await embeddingService.GenerateEmbeddingsAsync(
            textsList,
            cancellationToken: cancellationToken
        );

        return embeddings
               .Select(IReadOnlyList<float> (e) => e.ToArray())
               .ToList();
    }
}