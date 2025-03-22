using Npgsql;
using SacraScriptura.Web.Domain.Bibles;

namespace SacraScriptura.Web.Infrastructure.Database.Repositories;

public class BibleRecordRepository(
    string connectionString
) : IBibleRecordRepository
{
    public async Task<IReadOnlyList<BibleRecord>> GetAllAsync()
    {
        const string sql = """
            SELECT id, name, language_code, version, description, publisher_name, year
            FROM bibles
            ORDER BY name, version
            """;

        return await ExecuteQueryAsync(sql);
    }

    public async Task<BibleRecord?> GetByIdAsync(string id)
    {
        const string sql = """
            SELECT id, name, language_code, version, description, publisher_name, year
            FROM bibles
            WHERE id = @id
            """;

        var parameter = new NpgsqlParameter("@id", id);
        var results = await ExecuteQueryAsync(sql, parameter);

        return results.FirstOrDefault();
    }

    private async Task<IReadOnlyList<BibleRecord>> ExecuteQueryAsync(
        string sql,
        params NpgsqlParameter[] parameters
    )
    {
        var bibles = new List<BibleRecord>();

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddRange(parameters);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            bibles.Add(
                new BibleRecord(
                    Id: reader.GetString(0),
                    Name: reader.GetString(1),
                    LanguageCode: reader.GetString(2),
                    Version: reader.GetString(3),
                    Description: reader.IsDBNull(4) ? null : reader.GetString(4),
                    PublisherName: reader.IsDBNull(5) ? null : reader.GetString(5),
                    Year: reader.GetInt32(6)
                )
            );
        }

        return bibles.AsReadOnly();
    }
}