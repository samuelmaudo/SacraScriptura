using SacraScriptura.Admin.Domain.Bibles;
using SacraScriptura.Admin.Domain.Books;

namespace SacraScriptura.Admin.Application.Books;

public class BookUpdater(IBookRepository bookRepository)
{
    public async Task UpdateAsync(string id, BookDto bookDto)
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

    private static Book MapToEntity(BookDto dto)
    {
        return new Book
        {
            Id = dto.Id != null ? new BookId(dto.Id) : null,
            BibleId = new BibleId(dto.BibleId),
            Name = dto.Name,
            ShortName = dto.ShortName,
            Position = dto.Position,
        };
    }
}
