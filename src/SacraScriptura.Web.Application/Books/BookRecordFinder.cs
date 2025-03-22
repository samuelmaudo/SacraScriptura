using SacraScriptura.Web.Domain.Books;

namespace SacraScriptura.Web.Application.Books;

public class BookRecordFinder(
    IBookRecordRepository repository
)
{
    public async Task<BookRecord> FindAsync(string id)
    {
        var book = await repository.GetByIdAsync(id);

        if (book == null)
        {
            throw new KeyNotFoundException($"Book with ID {id} not found");
        }

        return book;
    }
}