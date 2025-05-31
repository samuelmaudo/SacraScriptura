using Microsoft.AspNetCore.Mvc;
using SacraScriptura.Web.Application.Divisions;
using SacraScriptura.Web.Domain.Divisions;

namespace SacraScriptura.Web.API.Controllers.Divisions;

/// <summary>
/// Controller for retrieving a division by its ID.
/// </summary>
[ApiController]
[Route("api/divisions/{id}")]
public class GetDivisionController(DivisionRecordFinder divisionFinder) : ControllerBase
{
    /// <summary>
    /// Gets a division by its ID.
    /// </summary>
    /// <param name="id">The ID of the division to retrieve.</param>
    /// <returns>The division with the specified ID.</returns>
    [HttpGet]
    [Tags("Divisions")]
    public async Task<ActionResult<DivisionRecord>> GetById(string id)
    {
        try
        {
            var division = await divisionFinder.FindAsync(id);
            return Ok(division);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
