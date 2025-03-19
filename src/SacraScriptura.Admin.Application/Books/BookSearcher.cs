using SacraScriptura.Admin.Domain.Bibles;
using SacraScriptura.Admin.Domain.Books;

namespace SacraScriptura.Admin.Application.Books;

public class BookSearcher(
    IBookRepository bookRepository
)
{
    public async Task<IEnumerable<BookDto>> SearchAsync()
    {
        var books = await bookRepository.GetAllAsync();
        return books.Select(MapToDto);
    }

    public async Task<IEnumerable<BookDto>> SearchByBibleIdAsync(string bibleId)
    {
        var bibleDomainId = new BibleId(bibleId);
        var books = await bookRepository.GetByBibleIdAsync(bibleDomainId);
        return books.Select(MapToDto);
    }

    private static BookDto MapToDto(Book book)
    {
        return new BookDto
        {
            Id = book.Id?.Value,
            BibleId = book.BibleId?.Value ?? string.Empty,
            Name = book.Name,
            ShortName = book.ShortName,
            Position = book.Position
        };
    }
}