namespace SacraScriptura.Web.Domain.Bibles;

public interface IBibleRecordRepository
{
    Task<IReadOnlyList<BibleRecord>> GetAllAsync();
    Task<BibleRecord?> GetByIdAsync(string id);
}
