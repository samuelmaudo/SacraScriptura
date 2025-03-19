namespace SacraScriptura.Admin.Domain.Bibles;

public class Bible
{
    public BibleId? Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string LanguageCode { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? PublisherName { get; set; }

    public int Year { get; set; }
}