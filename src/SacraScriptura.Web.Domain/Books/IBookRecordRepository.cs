namespace SacraScriptura.Web.Domain.Books;

public interface IBookRecordRepository
{
    Task<IReadOnlyList<BookRecord>> GetAllByBibleIdAsync(string bibleId);
    Task<BookRecord?> GetByIdAsync(string id);
}
