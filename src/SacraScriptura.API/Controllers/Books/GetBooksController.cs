using Microsoft.AspNetCore.Mvc;
using SacraScriptura.Application.Books;

namespace SacraScriptura.API.Controllers.Books;

/// <summary>
/// Controller for retrieving all books.
/// </summary>
[ApiController]
[Route("api/books")]
public class GetBooksController(
    BookService bookService
) : ControllerBase
{
    /// <summary>
    /// Gets all books.
    /// </summary>
    /// <returns>A collection of all books.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetAll()
    {
        var books = await bookService.GetAllBooksAsync();
        return Ok(books);
    }
}
