using Microsoft.AspNetCore.Mvc;
using SacraScriptura.Application.Bibles;

namespace SacraScriptura.API.Controllers;

/// <summary>
/// Controller for creating a new bible.
/// </summary>
[ApiController]
[Route("api/bibles")]
public class CreateBibleController(
    BibleService bibleService
) : ControllerBase
{
    /// <summary>
    /// Creates a new bible.
    /// </summary>
    /// <param name="bibleDto">The bible data to create.</param>
    /// <returns>The created bible.</returns>
    [HttpPost]
    public async Task<ActionResult<BibleDto>> Create(BibleDto bibleDto)
    {
        var createdBible = await bibleService.CreateBibleAsync(bibleDto);

        return CreatedAtAction(
            nameof(GetBibleController.GetById),
            "GetBible",
            new { id = createdBible.Id },
            createdBible
        );
    }
}
