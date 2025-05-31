using Microsoft.EntityFrameworkCore;
using SacraScriptura.Admin.Domain.Bibles;
using SacraScriptura.Admin.Domain.Books;

namespace SacraScriptura.Admin.Infrastructure.Database.Repositories;

public class BookRepository(ApplicationDbContext context) : IBookRepository
{
    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await context.Books.ToListAsync();
    }

    public async Task<Book?> GetByIdAsync(BookId id)
    {
        return await context.Books.FindAsync(id);
    }

    public async Task<IEnumerable<Book>> GetByBibleIdAsync(BibleId bibleId)
    {
        return await context
            .Books.Where(b => b.BibleId == bibleId)
            .OrderBy(b => b.Position)
            .ToListAsync();
    }

    public async Task AddAsync(Book book)
    {
        await context.Books.AddAsync(book);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Book book)
    {
        context.Books.Update(book);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(BookId id)
    {
        var book = await GetByIdAsync(id);
        if (book != null)
        {
            context.Books.Remove(book);
            await context.SaveChangesAsync();
        }
    }
}
