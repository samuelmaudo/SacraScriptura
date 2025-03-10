using Microsoft.AspNetCore.Mvc;
using SacraScriptura.Application.Books;

namespace SacraScriptura.API.Controllers;

/// <summary>
/// Controller for retrieving a book by its ID.
/// </summary>
[ApiController]
[Route("api/books/{id}")]
public class GetBookController(
    BookService bookService
) : ControllerBase
{
    /// <summary>
    /// Gets a book by its ID.
    /// </summary>
    /// <param name="id">The ID of the book to retrieve.</param>
    /// <returns>The book with the specified ID.</returns>
    [HttpGet]
    public async Task<ActionResult<BookDto>> GetById(string id)
    {
        try
        {
            var book = await bookService.GetBookByIdAsync(id);
            return Ok(book);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
