using Microsoft.AspNetCore.Mvc;
using SacraScriptura.Application.Bibles;

namespace SacraScriptura.API.Controllers.Bibles;

/// <summary>
/// Controller for creating a new bible.
/// </summary>
[ApiController]
[Route("api/bibles")]
public class CreateBibleController(
    BibleCreator bibleCreator
) : ControllerBase
{
    /// <summary>
    /// Creates a new bible.
    /// </summary>
    /// <param name="bibleDto">The bible data to create.</param>
    /// <returns>The created bible.</returns>
    [HttpPost]
    [Tags("Bibles")]
    public async Task<ActionResult<BibleDto>> Create(BibleDto bibleDto)
    {
        var createdBible = await bibleCreator.CreateAsync(bibleDto);

        return CreatedAtAction(
            nameof(GetBibleController.GetById),
            "GetBible",
            new { id = createdBible.Id },
            createdBible
        );
    }
}
