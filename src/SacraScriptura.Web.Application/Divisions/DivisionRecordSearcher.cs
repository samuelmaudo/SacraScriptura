using SacraScriptura.Web.Domain.Divisions;

namespace SacraScriptura.Web.Application.Divisions;

public class DivisionRecordSearcher(IDivisionRecordRepository repository)
{
    public async Task<IEnumerable<DivisionRecord>> SearchHierarchyByBookIdAsync(string bookId)
    {
        return await repository.GetHierarchyByBookIdAsync(bookId);
    }
}
