using SacraScriptura.Domain.Bibles;
using SacraScriptura.Domain.Books;

namespace SacraScriptura.Application.Books;

public class BookService(
    IBookRepository bookRepository
)
{
    public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
    {
        var books = await bookRepository.GetAllAsync();
        return books.Select(MapToDto);
    }

    public async Task<BookDto> GetBookByIdAsync(string id)
    {
        var bookId = new BookId(id);
        var book = await bookRepository.GetByIdAsync(bookId);

        if (book == null)
        {
            throw new KeyNotFoundException($"Book with ID {id} not found");
        }

        return MapToDto(book);
    }

    public async Task<IEnumerable<BookDto>> GetBooksByBibleIdAsync(string bibleId)
    {
        var bibleDomainId = new BibleId(bibleId);
        var books = await bookRepository.GetByBibleIdAsync(bibleDomainId);
        return books.Select(MapToDto);
    }

    public async Task<BookDto> CreateBookAsync(BookDto bookDto)
    {
        var book = MapToEntity(bookDto);
        book.Id = new BookId();

        await bookRepository.AddAsync(book);

        return MapToDto(book);
    }

    public async Task UpdateBookAsync(
        string id,
        BookDto bookDto
    )
    {
        var bookId = new BookId(id);
        var existingBook = await bookRepository.GetByIdAsync(bookId);

        if (existingBook == null)
        {
            throw new KeyNotFoundException($"Book with ID {id} not found");
        }

        var updatedBook = MapToEntity(bookDto);
        updatedBook.Id = bookId;

        await bookRepository.UpdateAsync(updatedBook);
    }

    public async Task DeleteBookAsync(string id)
    {
        var bookId = new BookId(id);
        var existingBook = await bookRepository.GetByIdAsync(bookId);

        if (existingBook == null)
        {
            throw new KeyNotFoundException($"Book with ID {id} not found");
        }

        await bookRepository.DeleteAsync(bookId);
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
