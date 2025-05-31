using SacraScriptura.Admin.Domain.Bibles;

namespace SacraScriptura.Admin.Domain.Books;

public class Book
{
    public BookId? Id { get; set; }

    public BibleId? BibleId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string ShortName { get; set; } = string.Empty;

    public int Position { get; set; }
}
