using Microsoft.AspNetCore.Mvc;
using SacraScriptura.Web.Application.Bibles;
using SacraScriptura.Web.Domain.Bibles;

namespace SacraScriptura.Web.API.Controllers.Bibles;

/// <summary>
/// Controller for retrieving all bibles.
/// </summary>
[ApiController]
[Route("api/bibles")]
public class GetBiblesController(BibleRecordSearcher bibleSearcher) : ControllerBase
{
    /// <summary>
    /// Gets all bibles.
    /// </summary>
    /// <returns>A collection of all bibles.</returns>
    [HttpGet]
    [Tags("Bibles")]
    public async Task<ActionResult<IEnumerable<BibleRecord>>> GetAll()
    {
        var bibles = await bibleSearcher.SearchAsync();
        return Ok(bibles);
    }
}
