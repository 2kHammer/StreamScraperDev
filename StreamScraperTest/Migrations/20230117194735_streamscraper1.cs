using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamScraperTest.Migrations
{
    public partial class streamscraper1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContentinformationsCountries_Countries_CountrieCountryId",
                table: "ContentinformationsCountries");

            migrationBuilder.RenameColumn(
                name: "CountrieCountryId",
                table: "ContentinformationsCountries",
                newName: "CountryId");

            migrationBuilder.RenameIndex(
                name: "IX_ContentinformationsCountries_CountrieCountryId",
                table: "ContentinformationsCountries",
                newName: "IX_ContentinformationsCountries_CountryId");

            migrationBuilder.AlterColumn<string>(
                name: "GenreName",
                table: "Genres",
                type: "char(20)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "CountryName",
                table: "Countries",
                type: "char(20)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ContenttypeDescription",
                table: "Contenttypes",
                type: "char(10)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Contents",
                type: "char(60)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Fullname",
                table: "Contentinformations",
                type: "char(60)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_ContentinformationsCountries_Countries_CountryId",
                table: "ContentinformationsCountries",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "CountryId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContentinformationsCountries_Countries_CountryId",
                table: "ContentinformationsCountries");

            migrationBuilder.RenameColumn(
                name: "CountryId",
                table: "ContentinformationsCountries",
                newName: "CountrieCountryId");

            migrationBuilder.RenameIndex(
                name: "IX_ContentinformationsCountries_CountryId",
                table: "ContentinformationsCountries",
                newName: "IX_ContentinformationsCountries_CountrieCountryId");

            migrationBuilder.AlterColumn<string>(
                name: "GenreName",
                table: "Genres",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(20)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "CountryName",
                table: "Countries",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(20)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "ContenttypeDescription",
                table: "Contenttypes",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(10)")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Contents",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(60)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Fullname",
                table: "Contentinformations",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(60)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_ContentinformationsCountries_Countries_CountrieCountryId",
                table: "ContentinformationsCountries",
                column: "CountrieCountryId",
                principalTable: "Countries",
                principalColumn: "CountryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
