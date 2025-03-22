using Microsoft.AspNetCore.Mvc;
using SacraScriptura.Web.Application.Books;
using SacraScriptura.Web.Application.Divisions;
using SacraScriptura.Web.Domain.Divisions;

namespace SacraScriptura.Web.API.Controllers.Divisions;

/// <summary>
/// Controller for retrieving divisions by book ID.
/// </summary>
[ApiController]
[Route("api/books/{bookId}/divisions")]
public class GetDivisionsByBookController(
    BookRecordFinder bookFinder,
    DivisionRecordSearcher divisionSearcher
) : ControllerBase
{
    /// <summary>
    /// Gets all divisions for a specific book in a hierarchical structure.
    /// </summary>
    /// <param name="bookId">The ID of the book to get divisions for.</param>
    /// <returns>A hierarchical collection of divisions for the specified book.</returns>
    [HttpGet]
    [Tags("Divisions")]
    public async Task<ActionResult<IEnumerable<DivisionRecord>>> GetByBookId(string bookId)
    {
        try
        {
            await bookFinder.FindAsync(bookId);
            var divisions = await divisionSearcher.SearchHierarchyByBookIdAsync(bookId);
            return Ok(divisions);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
