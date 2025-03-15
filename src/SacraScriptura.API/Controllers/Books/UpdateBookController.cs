using Microsoft.AspNetCore.Mvc;
using SacraScriptura.Application.Books;

namespace SacraScriptura.API.Controllers.Books;

/// <summary>
/// Controller for updating a book.
/// </summary>
[ApiController]
[Route("api/books/{id}")]
public class UpdateBookController(
    BookUpdater bookUpdater
) : ControllerBase
{
    /// <summary>
    /// Updates a book with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the book to update.</param>
    /// <param name="bookDto">The updated book data.</param>
    /// <returns>No content if successful.</returns>
    [HttpPut]
    [Tags("Books")]
    public async Task<IActionResult> Update(
        string id,
        BookDto bookDto
    )
    {
        try
        {
            await bookUpdater.UpdateAsync(id, bookDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
