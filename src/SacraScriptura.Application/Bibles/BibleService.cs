using SacraScriptura.Domain.Bibles;

namespace SacraScriptura.Application.Bibles;

public class BibleService(
    IBibleRepository bibleRepository
)
{
    public async Task<IEnumerable<BibleDto>> GetAllBiblesAsync()
    {
        var bibles = await bibleRepository.GetAllAsync();
        return bibles.Select(MapToDto);
    }

    public async Task<BibleDto> GetBibleByIdAsync(string id)
    {
        var bibleId = new BibleId(id);
        var bible = await bibleRepository.GetByIdAsync(bibleId);

        if (bible == null)
        {
            throw new KeyNotFoundException($"Bible with ID {id} not found");
        }

        return MapToDto(bible);
    }

    public async Task<BibleDto> CreateBibleAsync(BibleDto bibleDto)
    {
        var bible = MapToEntity(bibleDto);
        bible.Id = new BibleId();

        await bibleRepository.AddAsync(bible);

        return MapToDto(bible);
    }

    public async Task UpdateBibleAsync(
        string id,
        BibleDto bibleDto
    )
    {
        var bibleId = new BibleId(id);
        var existingBible = await bibleRepository.GetByIdAsync(bibleId);

        if (existingBible == null)
        {
            throw new KeyNotFoundException($"Bible with ID {id} not found");
        }

        var updatedBible = MapToEntity(bibleDto);
        updatedBible.Id = bibleId;

        await bibleRepository.UpdateAsync(updatedBible);
    }

    public async Task DeleteBibleAsync(string id)
    {
        var bibleId = new BibleId(id);
        var existingBible = await bibleRepository.GetByIdAsync(bibleId);

        if (existingBible == null)
        {
            throw new KeyNotFoundException($"Bible with ID {id} not found");
        }

        await bibleRepository.DeleteAsync(bibleId);
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