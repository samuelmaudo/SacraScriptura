using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SacraScriptura.Admin.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class CreateDivisionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "divisions",
                columns: table => new
                {
                    id = table.Column<string>(
                        type: "character varying(18)",
                        unicode: false,
                        maxLength: 18,
                        nullable: false,
                        collation: "C"
                    ),
                    book_id = table.Column<string>(
                        type: "character varying(18)",
                        unicode: false,
                        maxLength: 18,
                        nullable: false,
                        collation: "C"
                    ),
                    parent_id = table.Column<string>(
                        type: "character varying(18)",
                        unicode: false,
                        maxLength: 18,
                        nullable: true,
                        collation: "C"
                    ),
                    order = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    left_value = table.Column<int>(type: "integer", nullable: false),
                    right_value = table.Column<int>(type: "integer", nullable: false),
                    depth = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_divisions", x => x.id);
                    table.ForeignKey(
                        name: "FK_divisions_books_book_id",
                        column: x => x.book_id,
                        principalTable: "books",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_divisions_divisions_parent_id",
                        column: x => x.parent_id,
                        principalTable: "divisions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_divisions_book_id_depth",
                table: "divisions",
                columns: new[] { "book_id", "depth" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_divisions_book_id_left_value",
                table: "divisions",
                columns: new[] { "book_id", "left_value" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_divisions_book_id_right_value",
                table: "divisions",
                columns: new[] { "book_id", "right_value" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_divisions_parent_id",
                table: "divisions",
                column: "parent_id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "divisions");
        }
    }
}
