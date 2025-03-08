using Microsoft.AspNetCore.Mvc;
using SacraScriptura.Application.Bibles;

namespace SacraScriptura.API.Controllers;

[ApiController]
[Route("api/bibles")]
public class BiblesController(
    IBibleService bibleService
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BibleDto>>> GetAll()
    {
        var bibles = await bibleService.GetAllBiblesAsync();
        return Ok(bibles);
    }

    [HttpGet("{id}")]
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

    [HttpPost]
    public async Task<ActionResult<BibleDto>> Create(BibleDto bibleDto)
    {
        var createdBible = await bibleService.CreateBibleAsync(bibleDto);
        return CreatedAtAction(
            nameof(GetById),
            new { id = createdBible.Id },
            createdBible
        );
    }

    [HttpPut("{id}")]
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await bibleService.DeleteBibleAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}