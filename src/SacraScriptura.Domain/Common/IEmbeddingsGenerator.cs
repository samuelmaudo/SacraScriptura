namespace SacraScriptura.Domain.Common;

/// <summary>
/// Defines the service for generating vector representations (embeddings) from text.
/// </summary>
public interface IEmbeddingsGenerator
{
    /// <summary>
    /// Generates an embedding vector from a text.
    /// </summary>
    /// <param name="text">The text from which to generate the embedding.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A vector of float values representing the text embedding.</returns>
    Task<IReadOnlyList<float>> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates multiple embedding vectors from a collection of texts.
    /// </summary>
    /// <param name="texts">The texts from which to generate the embeddings.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of float vectors representing the text embeddings.</returns>
    Task<IReadOnlyList<IReadOnlyList<float>>> GenerateEmbeddingsAsync(
        IEnumerable<string> texts, 
        CancellationToken cancellationToken = default);
}
