namespace SacraScriptura.Web.Domain.Books;

public record BookRecord(
    string Id,
    string BibleId,
    string Name,
    string ShortName,
    int Position
);