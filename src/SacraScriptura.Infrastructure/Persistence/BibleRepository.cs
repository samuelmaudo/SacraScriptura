using Microsoft.EntityFrameworkCore;
using SacraScriptura.Domain;

namespace SacraScriptura.Infrastructure.Persistence;

public class BibleRepository(
    ApplicationDbContext context
) : IBibleRepository
{
    public async Task<IEnumerable<Bible>> GetAllAsync()
    {
        return await context.Bibles.ToListAsync();
    }

    public async Task<Bible?> GetByIdAsync(BibleId id)
    {
        return await context.Bibles.FindAsync(id);
    }

    public async Task<Bible?> GetByNameAndVersionAsync(
        string name,
        string version
    )
    {
        return await context.Bibles
                            .FirstOrDefaultAsync(b => b.Name == name && b.Version == version);
    }

    public async Task AddAsync(Bible bible)
    {
        await context.Bibles.AddAsync(bible);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Bible bible)
    {
        context.Bibles.Update(bible);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(BibleId id)
    {
        var bible = await GetByIdAsync(id);
        if (bible != null)
        {
            context.Bibles.Remove(bible);
            await context.SaveChangesAsync();
        }
    }
}