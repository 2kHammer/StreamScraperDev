using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamScraperTest.Migrations
{
    public partial class StreamScraper3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "urlPicture",
                table: "Contentinformations",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "urlPicture",
                table: "Contentinformations");
        }
    }
}
