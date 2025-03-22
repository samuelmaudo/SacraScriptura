namespace SacraScriptura.Web.Domain.Divisions;

public record DivisionRecord(
    string Id,
    string BookId,
    string? ParentId,
    int Order,
    string Title,
    int LeftValue,
    int RightValue,
    int Depth,
    IReadOnlyList<DivisionRecord> Children
);
