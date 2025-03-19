using SacraScriptura.Admin.Domain.Bibles;

namespace SacraScriptura.Admin.Application.Bibles;

public class BibleCreator(
    IBibleRepository bibleRepository
)
{
    public async Task<BibleDto> CreateAsync(BibleDto bibleDto)
    {
        var bible = MapToEntity(bibleDto);
        bible.Id = new BibleId();

        await bibleRepository.AddAsync(bible);

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
            Year = bible.Year
        };
    }

    private static Bible MapToEntity(BibleDto dto)
    {
        return new Bible
        {
            Id = dto.Id != null ? new BibleId(dto.Id) : null,
            Name = dto.Name,
            LanguageCode = dto.LanguageCode,
            Version = dto.Version,
            Description = dto.Description,
            PublisherName = dto.PublisherName,
            Year = dto.Year
        };
    }
}
