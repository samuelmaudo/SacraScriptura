using SacraScriptura.Admin.Domain.Bibles;

namespace SacraScriptura.Admin.Application.Bibles;

public class BibleFinder(IBibleRepository bibleRepository)
{
    public async Task<BibleDto> FindAsync(string id)
    {
        var bibleId = new BibleId(id);
        var bible = await bibleRepository.GetByIdAsync(bibleId);

        if (bible == null)
        {
            throw new KeyNotFoundException($"Bible with ID {id} not found");
        }

        return MapToDto(bible);
    }

    private static BibleDto MapToDto(Bible bible)
    {
        return new BibleDto
        {
            Id = bible.Id?.Value,
            Name = bible.Name,
            LanguageCode = bible.LanguageCode,
            Version = bible.Version,
            Description = bible.Description,
            PublisherName = bible.PublisherName,
            Year = bible.Year,
        };
    }
}
