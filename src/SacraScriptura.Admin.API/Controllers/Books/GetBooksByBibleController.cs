using Microsoft.AspNetCore.Mvc;
using SacraScriptura.Admin.Application.Bibles;
using SacraScriptura.Admin.Application.Books;

namespace SacraScriptura.Admin.API.Controllers.Books;

/// <summary>
/// Controller for retrieving books by bible ID.
/// </summary>
[ApiController]
[Route("api/bibles/{bibleId}/books")]
public class GetBooksByBibleController(BibleFinder bibleFinder, BookSearcher bookSearcher)
    : ControllerBase
{
    /// <summary>
    /// Gets all books for a specific bible.
    /// </summary>
    /// <param name="bibleId">The ID of the bible to get books for.</param>
    /// <returns>A collection of books for the specified bible.</returns>
    [HttpGet]
    [Tags("Books")]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetByBibleId(string bibleId)
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
