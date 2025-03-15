using Microsoft.AspNetCore.Mvc;
using SacraScriptura.Application.Bibles;

namespace SacraScriptura.API.Controllers.Bibles;

/// <summary>
/// Controller for retrieving all bibles.
/// </summary>
[ApiController]
[Route("api/bibles")]
public class GetBiblesController(
    BibleSearcher bibleSearcher
) : ControllerBase
{
    /// <summary>
    /// Gets all bibles.
    /// </summary>
    /// <returns>A collection of all bibles.</returns>
    [HttpGet]
    [Tags("Bibles")]
    public async Task<ActionResult<IEnumerable<BibleDto>>> GetAll()
    {
        var bibles = await bibleSearcher.SearchAsync();
        return Ok(bibles);
    }
}