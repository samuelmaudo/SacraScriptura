using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SacraScriptura.Admin.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class CreateBiblesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bibles",
                columns: table => new
                {
                    id = table.Column<string>(
                        type: "character varying(18)",
                        unicode: false,
                        maxLength: 18,
                        nullable: false,
                        collation: "C"
                    ),
                    name = table.Column<string>(
                        type: "character varying(63)",
                        maxLength: 63,
                        nullable: false
                    ),
                    language_code = table.Column<string>(
                        type: "character varying(5)",
                        unicode: false,
                        maxLength: 5,
                        nullable: false,
                        collation: "C"
                    ),
                    version = table.Column<string>(
                        type: "character varying(63)",
                        maxLength: 63,
                        nullable: false
                    ),
                    description = table.Column<string>(type: "text", nullable: true),
                    publisher_name = table.Column<string>(
                        type: "character varying(63)",
                        maxLength: 63,
                        nullable: true
                    ),
                    year = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bibles", x => x.id);
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_bibles_name_version",
                table: "bibles",
                columns: new[] { "name", "version" },
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "bibles");
        }
    }
}
