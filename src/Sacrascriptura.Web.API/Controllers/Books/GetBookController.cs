using Microsoft.AspNetCore.Mvc;
using SacraScriptura.Web.Application.Books;
using SacraScriptura.Web.Domain.Books;

namespace SacraScriptura.Web.API.Controllers.Books;

/// <summary>
/// Controller for retrieving a book by its ID.
/// </summary>
[ApiController]
[Route("api/books/{id}")]
public class GetBookController(BookRecordFinder bookFinder) : ControllerBase
{
    /// <summary>
    /// Gets a book by its ID.
    /// </summary>
    /// <param name="id">The ID of the book to retrieve.</param>
    /// <returns>The book with the specified ID.</returns>
    [HttpGet]
    [Tags("Books")]
    public async Task<ActionResult<BookRecord>> GetById(string id)
    {
        try
        {
            var book = await bookFinder.FindAsync(id);
            return Ok(book);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
