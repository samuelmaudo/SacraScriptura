using SacraScriptura.Admin.Domain.Books;

namespace SacraScriptura.Admin.Domain.Divisions;

/// <summary>
/// Repository interface for managing Division entities.
/// </summary>
public interface IDivisionRepository
{
    /// <summary>
    /// Gets all divisions.
    /// </summary>
    Task<IEnumerable<Division>> GetAllAsync();

    /// <summary>
    /// Gets a division by its ID.
    /// </summary>
    Task<Division?> GetByIdAsync(DivisionId id);

    /// <summary>
    /// Gets all divisions for a specific book.
    /// </summary>
    Task<IEnumerable<Division>> GetByBookIdAsync(BookId bookId);

    /// <summary>
    /// Gets all divisions for a specific book, ordered hierarchically.
    /// </summary>
    Task<IEnumerable<Division>> GetHierarchyByBookIdAsync(BookId bookId);

    /// <summary>
    /// Gets all children of a specific division.
    /// </summary>
    Task<IEnumerable<Division>> GetChildrenAsync(DivisionId parentId);

    /// <summary>
    /// Gets all ancestors of a specific division.
    /// </summary>
    Task<IEnumerable<Division>> GetAncestorsAsync(DivisionId divisionId);

    /// <summary>
    /// Gets all descendants of a specific division.
    /// </summary>
    Task<IEnumerable<Division>> GetDescendantsAsync(DivisionId divisionId);

    /// <summary>
    /// Adds a new division.
    /// </summary>
    Task AddAsync(Division division);

    /// <summary>
    /// Adds a new division as a child of a parent division.
    /// </summary>
    Task AddAsChildAsync(
        Division division,
        DivisionId parentId
    );

    /// <summary>
    /// Adds a new division as the first child of a parent division.
    /// </summary>
    Task AddAsFirstChildAsync(
        Division division,
        DivisionId parentId
    );

    /// <summary>
    /// Adds a new division as the last child of a parent division.
    /// </summary>
    Task AddAsLastChildAsync(
        Division division,
        DivisionId parentId
    );

    /// <summary>
    /// Adds a new division as a sibling before the specified division.
    /// </summary>
    Task AddBeforeAsync(
        Division division,
        DivisionId siblingId
    );

    /// <summary>
    /// Adds a new division as a sibling after the specified division.
    /// </summary>
    Task AddAfterAsync(
        Division division,
        DivisionId siblingId
    );

    /// <summary>
    /// Updates a division.
    /// </summary>
    Task UpdateAsync(Division division);

    /// <summary>
    /// Moves a division to be a child of a new parent.
    /// </summary>
    Task MoveToChildOfAsync(
        DivisionId divisionId,
        DivisionId newParentId
    );

    /// <summary>
    /// Moves a division to be before a sibling.
    /// </summary>
    Task MoveBeforeAsync(
        DivisionId divisionId,
        DivisionId siblingId
    );

    /// <summary>
    /// Moves a division to be after a sibling.
    /// </summary>
    Task MoveAfterAsync(
        DivisionId divisionId,
        DivisionId siblingId
    );

    /// <summary>
    /// Deletes a division and all its descendants.
    /// </summary>
    Task DeleteAsync(DivisionId id);

    /// <summary>
    /// Rebuilds the nested sets values for a book's divisions.
    /// </summary>
    Task RebuildTreeAsync(BookId bookId);
}