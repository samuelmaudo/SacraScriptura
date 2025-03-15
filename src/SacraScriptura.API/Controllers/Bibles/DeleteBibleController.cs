using Microsoft.AspNetCore.Mvc;
using SacraScriptura.Application.Bibles;

namespace SacraScriptura.API.Controllers.Bibles;

/// <summary>
/// Controller for deleting a bible.
/// </summary>
[ApiController]
[Route("api/bibles/{id}")]
public class DeleteBibleController(
    BibleDeleter bibleDeleter
) : ControllerBase
{
    /// <summary>
    /// Deletes a bible with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the bible to delete.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete]
    [Tags("Bibles")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await bibleDeleter.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
