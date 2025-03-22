namespace SacraScriptura.Web.Domain.Bibles;

public record BibleRecord(
    string Id, 
    string Name, 
    string LanguageCode, 
    string Version, 
    string? Description, 
    string? PublisherName, 
    int Year
);
