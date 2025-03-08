namespace SacraScriptura.Domain.Books;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAllAsync();
    Task<Book?> GetByIdAsync(BookId id);
    Task<IEnumerable<Book>> GetByBibleIdAsync(Bibles.BibleId bibleId);
    Task AddAsync(Book book);
    Task UpdateAsync(Book book);
    Task DeleteAsync(BookId id);
}
