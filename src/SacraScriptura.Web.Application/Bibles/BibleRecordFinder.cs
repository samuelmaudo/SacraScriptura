using SacraScriptura.Web.Domain;
using SacraScriptura.Web.Domain.Bibles;

namespace SacraScriptura.Web.Application.Bibles;

public class BibleRecordFinder(
    IBibleRecordRepository repository
)
{
    public async Task<BibleRecord> FindAsync(string id)
    {
        var bible = await repository.GetByIdAsync(id);

        if (bible == null)
        {
            throw new KeyNotFoundException($"Bible with ID {id} not found");
        }

        return bible;
    }
}