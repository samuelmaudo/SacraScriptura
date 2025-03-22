using SacraScriptura.Admin.Domain.Books;
using SacraScriptura.Admin.Domain.Divisions;

namespace SacraScriptura.Admin.Application.Divisions;

/// <summary>
/// Application service for managing book divisions.
/// </summary>
public class DivisionSearcher(
    DivisionService divisionService
)
{
    /// <summary>
    /// Gets the hierarchical structure of divisions for a book.
    /// </summary>
    public async Task<IEnumerable<DivisionDto>> SearchHierarchyByBookIdAsync(string bookId)
    {
        var bookIdObj = new BookId(bookId);
        var divisions = await divisionService.GetDivisionHierarchyForBookAsync(bookIdObj);

        return MapToDtoHierarchy(divisions);
    }

    /// <summary>
    /// Maps a collection of Division entities to DivisionDto objects, maintaining the hierarchy.
    /// </summary>
    private static List<DivisionDto> MapToDtoHierarchy(IEnumerable<Division> divisions)
    {
        var dtos = new List<DivisionDto>();

        foreach (var division in divisions)
        {
            var dto = MapToDto(division);

            if (division.Children.Any())
            {
                dto.Children = MapToDtoHierarchy(division.Children).ToList();
            }

            dtos.Add(dto);
        }

        return dtos;
    }

    /// <summary>
    /// Maps a Division entity to a DivisionDto.
    /// </summary>
    private static DivisionDto MapToDto(Division division)
    {
        return new DivisionDto
        {
            Id = division.Id?.ToString(),
            BookId = division.BookId?.ToString() ?? string.Empty,
            Title = division.Title,
            LeftValue = division.LeftValue,
            RightValue = division.RightValue,
            Depth = division.Depth,
            Order = division.Order,
            Children = new List<DivisionDto>(),
        };
    }
}