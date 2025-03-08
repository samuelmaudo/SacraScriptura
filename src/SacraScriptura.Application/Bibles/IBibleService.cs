namespace SacraScriptura.Application.Bibles;

public interface IBibleService
{
    Task<IEnumerable<BibleDto>> GetAllBiblesAsync();
    Task<BibleDto> GetBibleByIdAsync(string id);
    Task<BibleDto> CreateBibleAsync(BibleDto bibleDto);
    Task UpdateBibleAsync(string id, BibleDto bibleDto);
    Task DeleteBibleAsync(string id);
}
