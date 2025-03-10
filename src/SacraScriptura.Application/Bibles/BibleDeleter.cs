using SacraScriptura.Domain.Bibles;

namespace SacraScriptura.Application.Bibles;

public class BibleDeleter(
    IBibleRepository bibleRepository
)
{
    public async Task DeleteAsync(string id)
    {
        var bibleId = new BibleId(id);
        var existingBible = await bibleRepository.GetByIdAsync(bibleId);

        if (existingBible == null)
        {
            throw new KeyNotFoundException($"Bible with ID {id} not found");
        }

        await bibleRepository.DeleteAsync(bibleId);
    }
}
