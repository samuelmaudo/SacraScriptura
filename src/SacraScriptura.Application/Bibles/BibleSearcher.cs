using SacraScriptura.Domain.Bibles;

namespace SacraScriptura.Application.Bibles;

public class BibleSearcher(
    IBibleRepository bibleRepository
)
{
    public async Task<IEnumerable<BibleDto>> SearchAsync()
    {
        var bibles = await bibleRepository.GetAllAsync();
        return bibles.Select(MapToDto);
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
            Year = bible.Year
        };
    }
}