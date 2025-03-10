using Microsoft.AspNetCore.Mvc;
using SacraScriptura.Application.Bibles;

namespace SacraScriptura.API.Controllers.Bibles;

/// <summary>
/// Controller for retrieving all bibles.
/// </summary>
[ApiController]
[Route("api/bibles")]
public class GetBiblesController(
    BibleService bibleService
) : ControllerBase
{
    /// <summary>
    /// Gets all bibles.
    /// </summary>
    /// <returns>A collection of all bibles.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BibleDto>>> GetAll()
    {
        var bibles = await bibleService.GetAllBiblesAsync();
        return Ok(bibles);
    }
}