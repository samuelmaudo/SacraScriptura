using Microsoft.AspNetCore.Mvc;
using SacraScriptura.Web.Application.Bibles;
using SacraScriptura.Web.Application.Books;
using SacraScriptura.Web.Domain.Books;

namespace SacraScriptura.Web.API.Controllers.Books;

/// <summary>
/// Controller for retrieving books by bible ID.
/// </summary>
[ApiController]
[Route("api/bibles/{bibleId}/books")]
public class GetBooksByBibleController(
    BibleRecordFinder bibleFinder,
    BookRecordSearcher bookSearcher
) : ControllerBase
{
    /// <summary>
    /// Gets all books for a specific bible.
    /// </summary>
    /// <param name="bibleId">The ID of the bible to get books for.</param>
    /// <returns>A collection of books for the specified bible.</returns>
    [HttpGet]
    [Tags("Books")]
    public async Task<ActionResult<IEnumerable<BookRecord>>> GetByBibleId(string bibleId)
    {
        try
        {
            await bibleFinder.FindAsync(bibleId);
            var books = await bookSearcher.SearchByBibleIdAsync(bibleId);
            return Ok(books);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
