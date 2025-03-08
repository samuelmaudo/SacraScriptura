using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SacraScriptura.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class CreateBooksTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "books",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(18)", unicode: false, maxLength: 18, nullable: false, collation: "C"),
                    bible_id = table.Column<string>(type: "character varying(18)", unicode: false, maxLength: 18, nullable: false, collation: "C"),
                    name = table.Column<string>(type: "character varying(63)", maxLength: 63, nullable: false),
                    short_name = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    position = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_books", x => x.id);
                    table.ForeignKey(
                        name: "FK_books_bibles_bible_id",
                        column: x => x.bible_id,
                        principalTable: "bibles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_books_bible_id_name",
                table: "books",
                columns: new[] { "bible_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_books_bible_id_position",
                table: "books",
                columns: new[] { "bible_id", "position" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "books");
        }
    }
}
