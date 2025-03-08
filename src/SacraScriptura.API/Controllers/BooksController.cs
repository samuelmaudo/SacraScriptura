using Microsoft.AspNetCore.Mvc;
using SacraScriptura.Application.Books;

namespace SacraScriptura.API.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController(
    BookService bookService
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetAll()
    {
        var books = await bookService.GetAllBooksAsync();
        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetById(string id)
    {
        try
        {
            var book = await bookService.GetBookByIdAsync(id);
            return Ok(book);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("bible/{bibleId}")]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetByBibleId(string bibleId)
    {
        var books = await bookService.GetBooksByBibleIdAsync(bibleId);
        return Ok(books);
    }

    [HttpPost]
    public async Task<ActionResult<BookDto>> Create(BookDto bookDto)
    {
        var createdBook = await bookService.CreateBookAsync(bookDto);
        return CreatedAtAction(
            nameof(GetById),
            new { id = createdBook.Id },
            createdBook
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        string id,
        BookDto bookDto
    )
    {
        try
        {
            await bookService.UpdateBookAsync(id, bookDto);
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
            await bookService.DeleteBookAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
