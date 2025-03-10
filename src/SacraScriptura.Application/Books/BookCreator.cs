using SacraScriptura.Domain.Bibles;
using SacraScriptura.Domain.Books;

namespace SacraScriptura.Application.Books;

public class BookCreator(
    IBookRepository bookRepository
)
{
    public async Task<BookDto> CreateAsync(BookDto bookDto)
    {
        var book = MapToEntity(bookDto);
        book.Id = new BookId();

        await bookRepository.AddAsync(book);

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

    private static Book MapToEntity(BookDto dto)
    {
        return new Book
        {
            Id = dto.Id != null ? new BookId(dto.Id) : null,
            BibleId = new BibleId(dto.BibleId),
            Name = dto.Name,
            ShortName = dto.ShortName,
            Position = dto.Position
        };
    }
}
