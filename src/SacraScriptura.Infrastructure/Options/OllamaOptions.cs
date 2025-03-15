namespace SacraScriptura.Infrastructure.Options;

/// <summary>
/// Configuration options for the Ollama service.
/// </summary>
public class OllamaOptions
{
    /// <summary>
    /// The section in the configuration file where these options are defined.
    /// </summary>
    public const string SectionName = "Ollama";

    /// <summary>
    /// Base URL of the Ollama server.
    /// </summary>
    public string BaseUrl { get; set; } = "http://localhost:11434";

    /// <summary>
    /// Name of the model to use for generating embeddings.
    /// </summary>
    public string EmbeddingsModel { get; set; } = "mistral-embeddings";

    /// <summary>
    /// Name of the model to use for generating summaries.
    /// </summary>
    public string SummarizationModel { get; set; } = "e5-base-multilingual";

    /// <summary>
    /// Maximum timeout for requests to Ollama in seconds.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 60;
}
