using Npgsql;
using SacraScriptura.Web.Domain.Books;

namespace SacraScriptura.Web.Infrastructure.Database.Repositories;

public class BookRecordRepository(string connectionString) : IBookRecordRepository
{
    public async Task<IReadOnlyList<BookRecord>> GetAllByBibleIdAsync(string bibleId)
    {
        const string sql = """
            SELECT id, bible_id, name, short_name, position
            FROM books
            WHERE bible_id = @bibleId
            ORDER BY position
            """;

        var parameter = new NpgsqlParameter("@bibleId", bibleId);

        return await ExecuteQueryAsync(sql, parameter);
    }

    public async Task<BookRecord?> GetByIdAsync(string id)
    {
        const string sql = """
            SELECT id, bible_id, name, short_name, position
            FROM books
            WHERE id = @id
            """;

        var parameter = new NpgsqlParameter("@id", id);
        var results = await ExecuteQueryAsync(sql, parameter);

        return results.FirstOrDefault();
    }

    private async Task<IReadOnlyList<BookRecord>> ExecuteQueryAsync(
        string sql,
        params NpgsqlParameter[] parameters
    )
    {
        var books = new List<BookRecord>();

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddRange(parameters);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            books.Add(
                new BookRecord(
                    Id: reader.GetString(0),
                    BibleId: reader.GetString(1),
                    Name: reader.GetString(2),
                    ShortName: reader.GetString(3),
                    Position: reader.GetInt32(4)
                )
            );
        }

        return books.AsReadOnly();
    }
}
