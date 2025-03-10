using Microsoft.AspNetCore.Mvc;
using SacraScriptura.Application.Books;

namespace SacraScriptura.API.Controllers.Books;

/// <summary>
/// Controller for creating a new book.
/// </summary>
[ApiController]
[Route("api/books")]
public class CreateBookController(
    BookCreator bookCreator
) : ControllerBase
{
    /// <summary>
    /// Creates a new book.
    /// </summary>
    /// <param name="bookDto">The book data to create.</param>
    /// <returns>The created book.</returns>
    [HttpPost]
    public async Task<ActionResult<BookDto>> Create(BookDto bookDto)
    {
        var createdBook = await bookCreator.CreateAsync(bookDto);

        return CreatedAtAction(
            nameof(GetBookController.GetById),
            "GetBook",
            new { id = createdBook.Id },
            createdBook
        );
    }
}
