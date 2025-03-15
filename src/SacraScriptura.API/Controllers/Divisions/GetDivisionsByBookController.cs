using Microsoft.AspNetCore.Mvc;
using SacraScriptura.Application.Divisions;

namespace SacraScriptura.API.Controllers.Divisions;

/// <summary>
/// Controller for retrieving divisions by book ID.
/// </summary>
[ApiController]
[Route("api/books/{bookId}/divisions")]
public class GetDivisionsByBookController(
    DivisionService divisionService
) : ControllerBase
{
    /// <summary>
    /// Gets all divisions for a book in hierarchical order.
    /// </summary>
    /// <param name="bookId">The ID of the book to get divisions for.</param>
    /// <returns>A collection of divisions in hierarchical order.</returns>
    [HttpGet]
    [Tags("Divisions")]
    public async Task<ActionResult<IEnumerable<DivisionDto>>> GetHierarchyByBookId(string bookId)
    {
        try
        {
            var divisions = await divisionService.GetDivisionHierarchyForBookAsync(bookId);
            return Ok(divisions);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while retrieving the divisions.");
        }
    }
}