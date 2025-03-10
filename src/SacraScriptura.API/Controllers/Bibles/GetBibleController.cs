using Microsoft.AspNetCore.Mvc;
using SacraScriptura.Application.Bibles;

namespace SacraScriptura.API.Controllers.Bibles;

/// <summary>
/// Controller for retrieving a bible by its ID.
/// </summary>
[ApiController]
[Route("api/bibles/{id}")]
public class GetBibleController(
    BibleService bibleService
) : ControllerBase
{
    /// <summary>
    /// Gets a bible by its ID.
    /// </summary>
    /// <param name="id">The ID of the bible to retrieve.</param>
    /// <returns>The bible with the specified ID.</returns>
    [HttpGet]
    public async Task<ActionResult<BibleDto>> GetById(string id)
    {
        try
        {
            var bible = await bibleService.GetBibleByIdAsync(id);
            return Ok(bible);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
