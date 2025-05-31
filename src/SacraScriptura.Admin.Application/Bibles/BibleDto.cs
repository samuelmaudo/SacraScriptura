using System.ComponentModel.DataAnnotations;

namespace SacraScriptura.Admin.Application.Bibles;

public class BibleDto
{
    public string? Id { get; set; }

    [Required]
    [StringLength(63)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(5)]
    public string LanguageCode { get; set; } = string.Empty;

    [Required]
    [StringLength(63)]
    public string Version { get; set; } = string.Empty;

    public string? Description { get; set; }

    [StringLength(63)]
    public string? PublisherName { get; set; }

    public int Year { get; set; }
}
