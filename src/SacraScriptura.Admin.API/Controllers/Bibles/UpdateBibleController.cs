using Microsoft.AspNetCore.Mvc;
using SacraScriptura.Admin.Application.Bibles;

namespace SacraScriptura.Admin.API.Controllers.Bibles;

/// <summary>
/// Controller for updating a bible.
/// </summary>
[ApiController]
[Route("api/bibles/{id}")]
public class UpdateBibleController(
    BibleUpdater bibleUpdater
) : ControllerBase
{
    /// <summary>
    /// Updates a bible with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the bible to update.</param>
    /// <param name="bibleDto">The updated bible data.</param>
    /// <returns>No content if successful.</returns>
    [HttpPut]
    [Tags("Bibles")]
    public async Task<IActionResult> Update(
        string id,
        BibleDto bibleDto
    )
    {
        try
        {
            await bibleUpdater.UpdateAsync(id, bibleDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
