using System.ComponentModel.DataAnnotations;

namespace SacraScriptura.Application.Divisions;

/// <summary>
/// Data transfer object for Division entities.
/// </summary>
public class DivisionDto
{
    /// <summary>
    /// Gets or sets the unique identifier for this division.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the book identifier that this division belongs to.
    /// </summary>
    [Required]
    public string BookId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the title of this division.
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the left value in the nested set model.
    /// </summary>
    public int LeftValue { get; set; }

    /// <summary>
    /// Gets or sets the right value in the nested set model.
    /// </summary>
    public int RightValue { get; set; }

    /// <summary>
    /// Gets or sets the depth level in the hierarchy.
    /// </summary>
    public int Depth { get; set; }

    /// <summary>
    /// Gets or sets the order among siblings.
    /// </summary>
    public int Order { get; set; }
    
    /// <summary>
    /// Gets or sets the collection of child divisions.
    /// </summary>
    public ICollection<DivisionDto>? Children { get; set; }
}
