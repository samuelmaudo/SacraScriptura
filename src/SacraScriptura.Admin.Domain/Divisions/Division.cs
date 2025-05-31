using System.Collections.ObjectModel;
using SacraScriptura.Admin.Domain.Books;

namespace SacraScriptura.Admin.Domain.Divisions;

/// <summary>
/// Represents a division within a book (part, section, chapter, etc.) using the Nested Sets pattern.
/// </summary>
public class Division
{
    /// <summary>
    /// Gets or sets the unique identifier for this division.
    /// </summary>
    public DivisionId? Id { get; set; }

    /// <summary>
    /// Gets or sets the book this division belongs to.
    /// </summary>
    public BookId? BookId { get; set; }

    /// <summary>
    /// Gets or sets the parent division's unique identifier.
    /// </summary>
    public DivisionId? ParentId { get; set; }

    /// <summary>
    /// Gets or sets the order of this division among its book.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets the title of this division.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the left boundary value in the Nested Sets model.
    /// </summary>
    public int LeftValue { get; set; }

    /// <summary>
    /// Gets or sets the right boundary value in the Nested Sets model.
    /// </summary>
    public int RightValue { get; set; }

    /// <summary>
    /// Gets or sets the depth of this node in the tree hierarchy.
    /// </summary>
    public int Depth { get; set; }

    /// <summary>
    /// Determines if this division is a leaf node (has no children).
    /// </summary>
    public bool IsLeaf => RightValue - LeftValue == 1;

    /// <summary>
    /// Calculates the number of descendants this division has.
    /// </summary>
    public int DescendantCount => (RightValue - LeftValue - 1) / 2;

    /// <summary>
    /// Gets or sets the parent division.
    /// </summary>
    public Division? Parent { get; set; }

    /// <summary>
    /// Gets the collection of direct children of this division.
    /// </summary>
    public ICollection<Division> Children { get; private set; } = new Collection<Division>();

    /// <summary>
    /// Gets the collection of all descendants of this division (children, grandchildren, etc.).
    /// </summary>
    public ICollection<Division> Descendants { get; private set; } = new Collection<Division>();
}
