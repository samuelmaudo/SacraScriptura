namespace SacraScriptura.Domain;

public interface IBibleRepository
{
    Task<IEnumerable<Bible>> GetAllAsync();
    Task<Bible?> GetByIdAsync(BibleId id);
    Task<Bible?> GetByNameAndVersionAsync(
        string name,
        string version
    );
    Task AddAsync(Bible bible);
    Task UpdateAsync(Bible bible);
    Task DeleteAsync(BibleId id);
}