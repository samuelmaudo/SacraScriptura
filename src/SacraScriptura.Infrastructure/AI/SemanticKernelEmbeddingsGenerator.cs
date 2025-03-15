using Microsoft.SemanticKernel.Embeddings;
using SacraScriptura.Domain.Common;

namespace SacraScriptura.Infrastructure.AI;

/// <summary>
/// Implementation of the embeddings generator using Ollama with Semantic Kernel.
/// </summary>
public class SemanticKernelEmbeddingsGenerator(
    ITextEmbeddingGenerationService embeddingService
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