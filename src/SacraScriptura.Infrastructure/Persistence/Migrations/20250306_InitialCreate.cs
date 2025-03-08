using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SacraScriptura.Infrastructure.Persistence.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "bibles",
            columns: table => new
            {
                id = table.Column<string>(type: "character varying(18)", maxLength: 18, unicode: false, collation: "C", nullable: false),
                name = table.Column<string>(type: "character varying(63)", maxLength: 255, unicode: true, nullable: false),
                language_code = table.Column<string>(type: "character varying(5)", maxLength: 50, unicode: false, collation: "C", nullable: false),
                version = table.Column<string>(type: "character varying(63)", maxLength: 100, unicode: true, nullable: false),
                description = table.Column<string>(type: "text", unicode: true, nullable: true),
                publisher_name = table.Column<string>(type: "character varying(63)", maxLength: 255, unicode: true, nullable: true),
                year = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_bibles", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_bibles_name_version",
            table: "bibles",
            columns: new[] { "name", "version" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "bibles");
    }
}
