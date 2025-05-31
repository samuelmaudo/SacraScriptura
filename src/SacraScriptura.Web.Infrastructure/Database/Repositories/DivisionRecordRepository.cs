using Npgsql;
using SacraScriptura.Web.Domain.Divisions;

namespace SacraScriptura.Web.Infrastructure.Database.Repositories;

public class DivisionRecordRepository(string connectionString) : IDivisionRecordRepository
{
    public async Task<DivisionRecord?> GetByIdAsync(string id)
    {
        const string sql = """
            SELECT id, book_id, parent_id, "order", title, left_value, right_value, depth
            FROM divisions
            WHERE id = @id
            """;

        var parameter = new NpgsqlParameter("@id", id);
        var results = await ExecuteQueryAsync(sql, parameter);

        return results.FirstOrDefault();
    }

    public async Task<IReadOnlyList<DivisionRecord>> GetHierarchyByBookIdAsync(string bookId)
    {
        const string sql = """
            SELECT id, book_id, parent_id, "order", title, left_value, right_value, depth
            FROM divisions
            WHERE book_id = @bookId
            ORDER BY left_value
            """;

        var parameter = new NpgsqlParameter("@bookId", bookId);
        var allDivisions = await ExecuteQueryAsync(sql, parameter);

        return BuildHierarchy(allDivisions);
    }

    private async Task<IReadOnlyList<DivisionRecord>> ExecuteQueryAsync(
        string sql,
        params NpgsqlParameter[] parameters
    )
    {
        var divisions = new List<DivisionRecord>();

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddRange(parameters);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            divisions.Add(
                new DivisionRecord(
                    Id: reader.GetString(0),
                    BookId: reader.GetString(1),
                    ParentId: reader.IsDBNull(2) ? null : reader.GetString(2),
                    Order: reader.GetInt32(3),
                    Title: reader.GetString(4),
                    LeftValue: reader.GetInt32(5),
                    RightValue: reader.GetInt32(6),
                    Depth: reader.GetInt32(7),
                    Children: Array.Empty<DivisionRecord>()
                )
            );
        }

        return divisions.AsReadOnly();
    }

    private static IReadOnlyList<DivisionRecord> BuildHierarchy(
        IReadOnlyList<DivisionRecord> allDivisions
    )
    {
        var stack = new Stack<MutableDivision>();
        var roots = new List<MutableDivision>();

        foreach (var division in allDivisions)
        {
            var node = new MutableDivision(division);

            // Remove nodes that are no longer ancestors of the current node
            while (stack.Count > 0 && stack.Peek().RightValue < node.RightValue)
            {
                stack.Pop();
            }

            if (stack.Count > 0)
                stack.Peek().Children.Add(node);
            else
                roots.Add(node);

            stack.Push(node);
        }

        return roots.Select(r => r.ToDivisionRecord()).ToList().AsReadOnly();
    }

    private class MutableDivision(DivisionRecord division)
    {
        private string Id { get; } = division.Id;
        private string BookId { get; } = division.BookId;
        private string? ParentId { get; } = division.ParentId;
        private int Order { get; } = division.Order;
        private string Title { get; } = division.Title;
        private int LeftValue { get; } = division.LeftValue;
        public int RightValue { get; } = division.RightValue;
        private int Depth { get; } = division.Depth;
        public List<MutableDivision> Children { get; } = [];

        public DivisionRecord ToDivisionRecord() =>
            new(
                Id,
                BookId,
                ParentId,
                Order,
                Title,
                LeftValue,
                RightValue,
                Depth,
                Children.Select(child => child.ToDivisionRecord()).ToList().AsReadOnly()
            );
    }
}
