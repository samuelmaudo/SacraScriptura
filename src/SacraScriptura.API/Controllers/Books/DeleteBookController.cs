using Microsoft.AspNetCore.Mvc;
using SacraScriptura.Application.Books;

namespace SacraScriptura.API.Controllers.Books;

/// <summary>
/// Controller for deleting a book.
/// </summary>
[ApiController]
[Route("api/books/{id}")]
public class DeleteBookController(
    BookService bookService
) : ControllerBase
{
    /// <summary>
    /// Deletes a book with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the book to delete.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await bookService.DeleteBookAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
