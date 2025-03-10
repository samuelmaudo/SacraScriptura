using Microsoft.AspNetCore.Mvc;
using SacraScriptura.Application.Bibles;

namespace SacraScriptura.API.Controllers;

/// <summary>
/// Controller for updating a bible.
/// </summary>
[ApiController]
[Route("api/bibles/{id}")]
public class UpdateBibleController(
    BibleService bibleService
) : ControllerBase
{
    /// <summary>
    /// Updates a bible with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the bible to update.</param>
    /// <param name="bibleDto">The updated bible data.</param>
    /// <returns>No content if successful.</returns>
    [HttpPut]
    public async Task<IActionResult> Update(
        string id,
        BibleDto bibleDto
    )
    {
        try
        {
            await bibleService.UpdateBibleAsync(id, bibleDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
