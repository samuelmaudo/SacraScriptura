using SacraScriptura.Web.Domain.Bibles;

namespace SacraScriptura.Web.Application.Bibles;

public class BibleRecordSearcher(IBibleRecordRepository repository)
{
    public async Task<IEnumerable<BibleRecord>> SearchAsync()
    {
        return await repository.GetAllAsync();
    }
}
