using System.ComponentModel.DataAnnotations;

namespace SacraScriptura.Admin.Application.Books;

public class BookDto
{
    public string? Id { get; set; }

    [Required]
    public string BibleId { get; set; } = string.Empty;

    [Required]
    [StringLength(63)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(15)]
    public string ShortName { get; set; } = string.Empty;

    [Required]
    public int Position { get; set; }
}