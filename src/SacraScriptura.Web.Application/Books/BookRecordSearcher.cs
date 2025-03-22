using SacraScriptura.Web.Domain.Books;

namespace SacraScriptura.Web.Application.Books;

public class BookRecordSearcher(
    IBookRecordRepository repository
)
{
    public async Task<IEnumerable<BookRecord>> SearchByBibleIdAsync(string bibleId)
    {
        return await repository.GetAllByBibleIdAsync(bibleId);
    }
}