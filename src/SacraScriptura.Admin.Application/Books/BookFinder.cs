using SacraScriptura.Admin.Domain.Books;

namespace SacraScriptura.Admin.Application.Books;

public class BookFinder(
    IBookRepository bookRepository
)
{
    public async Task<BookDto> FindAsync(string id)
    {
        var bookId = new BookId(id);
        var book = await bookRepository.GetByIdAsync(bookId);

        if (book == null)
        {
            throw new KeyNotFoundException($"Book with ID {id} not found");
        }

        return MapToDto(book);
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