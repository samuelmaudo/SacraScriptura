using SacraScriptura.Admin.Domain.Books;

namespace SacraScriptura.Admin.Application.Books;

public class BookDeleter(
    IBookRepository bookRepository
)
{
    public async Task DeleteAsync(string id)
    {
        var bookId = new BookId(id);
        var existingBook = await bookRepository.GetByIdAsync(bookId);

        if (existingBook == null)
        {
            throw new KeyNotFoundException($"Book with ID {id} not found");
        }

        await bookRepository.DeleteAsync(bookId);
    }
}
