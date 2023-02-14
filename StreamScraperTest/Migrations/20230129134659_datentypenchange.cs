using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamScraperTest.Migrations
{
    public partial class datentypenchange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GenreName",
                table: "Genres",
                type: "char(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(20)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GenreName",
                table: "Genres",
                type: "char(20)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(30)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
