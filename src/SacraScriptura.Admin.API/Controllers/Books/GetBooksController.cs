using Microsoft.AspNetCore.Mvc;
using SacraScriptura.Admin.Application.Books;

namespace SacraScriptura.Admin.API.Controllers.Books;

/// <summary>
/// Controller for retrieving all books.
/// </summary>
[ApiController]
[Route("api/books")]
public class GetBooksController(
    BookSearcher bookSearcher
) : ControllerBase
{
    /// <summary>
    /// Gets all books.
    /// </summary>
    /// <returns>A collection of all books.</returns>
    [HttpGet]
    [Tags("Books")]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetAll()
    {
        var books = await bookSearcher.SearchAsync();
        return Ok(books);
    }
}
