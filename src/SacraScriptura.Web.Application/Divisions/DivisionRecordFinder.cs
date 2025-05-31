using SacraScriptura.Web.Domain.Divisions;

namespace SacraScriptura.Web.Application.Divisions;

public class DivisionRecordFinder(IDivisionRecordRepository repository)
{
    public async Task<DivisionRecord> FindAsync(string id)
    {
        var division = await repository.GetByIdAsync(id);

        if (division == null)
        {
            throw new KeyNotFoundException($"Division with ID {id} not found");
        }

        return division;
    }
}
