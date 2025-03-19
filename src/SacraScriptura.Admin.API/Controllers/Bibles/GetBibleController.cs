using Microsoft.AspNetCore.Mvc;
using SacraScriptura.Admin.Application.Bibles;

namespace SacraScriptura.Admin.API.Controllers.Bibles;

/// <summary>
/// Controller for retrieving a bible by its ID.
/// </summary>
[ApiController]
[Route("api/bibles/{id}")]
public class GetBibleController(
    BibleFinder bibleFinder
) : ControllerBase
{
    /// <summary>
    /// Gets a bible by its ID.
    /// </summary>
    /// <param name="id">The ID of the bible to retrieve.</param>
    /// <returns>The bible with the specified ID.</returns>
    [HttpGet]
    [Tags("Bibles")]
    public async Task<ActionResult<BibleDto>> GetById(string id)
    {
        try
        {
            var bible = await bibleFinder.FindAsync(id);
            return Ok(bible);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
