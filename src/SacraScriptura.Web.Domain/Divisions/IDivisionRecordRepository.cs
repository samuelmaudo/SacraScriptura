namespace SacraScriptura.Web.Domain.Divisions;

public interface IDivisionRecordRepository
{
    Task<DivisionRecord?> GetByIdAsync(string id);
    Task<IReadOnlyList<DivisionRecord>> GetHierarchyByBookIdAsync(string bookId);
}
