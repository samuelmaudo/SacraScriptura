using Microsoft.AspNetCore.Mvc;
using SacraScriptura.Application.Books;

namespace SacraScriptura.API.Controllers.Books;

/// <summary>
/// Controller for retrieving books by bible ID.
/// </summary>
[ApiController]
[Route("api/bibles/{bibleId}/books")]
public class GetBooksByBibleController(
    BookSearcher bookSearcher
) : ControllerBase
{
    /// <summary>
    /// Gets all books for a specific bible.
    /// </summary>
    /// <param name="bibleId">The ID of the bible to get books for.</param>
    /// <returns>A collection of books for the specified bible.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetByBibleId(string bibleId)
    {
        var books = await bookSearcher.SearchByBibleIdAsync(bibleId);
        return Ok(books);
    }
}
