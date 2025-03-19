using SacraScriptura.Admin.Domain.Books;

namespace SacraScriptura.Admin.Domain.Divisions;

/// <summary>
/// Service for managing book divisions using the Nested Sets pattern.
/// </summary>
public class DivisionService(
    IDivisionRepository divisionRepository
)
{
    /// <summary>
    /// Gets all divisions for a book.
    /// </summary>
    public async Task<IEnumerable<Division>> GetDivisionsForBookAsync(BookId bookId)
    {
        return await divisionRepository.GetByBookIdAsync(bookId);
    }

    /// <summary>
    /// Gets the hierarchical structure of divisions for a book.
    /// </summary>
    public async Task<IEnumerable<Division>> GetDivisionHierarchyForBookAsync(BookId bookId)
    {
        return await divisionRepository.GetHierarchyByBookIdAsync(bookId);
    }

    /// <summary>
    /// Creates a new root division for a book.
    /// </summary>
    public async Task<Division> CreateRootDivisionAsync(
        BookId bookId,
        string title
    )
    {
        var division = new Division
        {
            Id = new DivisionId(),
            BookId = bookId,
            Title = title,
            LeftValue = 1,
            RightValue = 2,
            Depth = 0,
            Order = 0
        };

        await divisionRepository.AddAsync(division);
        return division;
    }

    /// <summary>
    /// Creates a new division as a child of an existing division.
    /// </summary>
    public async Task<Division> CreateChildDivisionAsync(
        DivisionId parentId,
        string title
    )
    {
        var parent = await divisionRepository.GetByIdAsync(parentId)
                     ?? throw new ArgumentException("Parent division not found", nameof(parentId));

        var division = new Division
        {
            Id = new DivisionId(),
            BookId = parent.BookId,
            Title = title,
            Depth = parent.Depth + 1
        };

        await divisionRepository.AddAsLastChildAsync(division, parentId);
        return division;
    }

    /// <summary>
    /// Creates a new division as a sibling after an existing division.
    /// </summary>
    public async Task<Division> CreateSiblingDivisionAsync(
        DivisionId siblingId,
        string title
    )
    {
        var sibling = await divisionRepository.GetByIdAsync(siblingId)
                      ?? throw new ArgumentException("Sibling division not found", nameof(siblingId));

        var division = new Division
        {
            Id = new DivisionId(),
            BookId = sibling.BookId,
            Title = title,
            Depth = sibling.Depth
        };

        await divisionRepository.AddAfterAsync(division, siblingId);
        return division;
    }

    /// <summary>
    /// Updates a division's title.
    /// </summary>
    public async Task UpdateDivisionTitleAsync(
        DivisionId divisionId,
        string newTitle
    )
    {
        var division = await divisionRepository.GetByIdAsync(divisionId)
                       ?? throw new ArgumentException("Division not found", nameof(divisionId));

        division.Title = newTitle;
        await divisionRepository.UpdateAsync(division);
    }

    /// <summary>
    /// Moves a division to be a child of a new parent.
    /// </summary>
    public async Task MoveDivisionToParentAsync(
        DivisionId divisionId,
        DivisionId newParentId
    )
    {
        await divisionRepository.MoveToChildOfAsync(divisionId, newParentId);
    }

    /// <summary>
    /// Moves a division to be before another division.
    /// </summary>
    public async Task MoveDivisionBeforeAsync(
        DivisionId divisionId,
        DivisionId targetId
    )
    {
        await divisionRepository.MoveBeforeAsync(divisionId, targetId);
    }

    /// <summary>
    /// Moves a division to be after another division.
    /// </summary>
    public async Task MoveDivisionAfterAsync(
        DivisionId divisionId,
        DivisionId targetId
    )
    {
        await divisionRepository.MoveAfterAsync(divisionId, targetId);
    }

    /// <summary>
    /// Deletes a division and all its descendants.
    /// </summary>
    public async Task DeleteDivisionAsync(DivisionId divisionId)
    {
        await divisionRepository.DeleteAsync(divisionId);
    }

    /// <summary>
    /// Rebuilds the nested sets values for a book's divisions.
    /// </summary>
    public async Task RebuildDivisionTreeAsync(BookId bookId)
    {
        await divisionRepository.RebuildTreeAsync(bookId);
    }
}