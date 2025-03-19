using SacraScriptura.Admin.Domain.Bibles;

namespace SacraScriptura.Admin.Application.Bibles;

public class BibleUpdater(
    IBibleRepository bibleRepository
)
{
    public async Task UpdateAsync(
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