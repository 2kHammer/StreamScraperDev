using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamScraperTest.Migrations
{
    public partial class scrapertest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Contents",
                columns: table => new
                {
                    ContentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Year = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contents", x => x.ContentId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Contenttypes",
                columns: table => new
                {
                    ContenttypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ContenttypeDescription = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contenttypes", x => x.ContenttypeId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    CountryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CountryName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.CountryId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    GenreId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GenreName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.GenreId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Contentinformations",
                columns: table => new
                {
                    ContentinformationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Rating = table.Column<float>(type: "float", nullable: true),
                    Fullname = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Lastyear = table.Column<int>(type: "int", nullable: true),
                    DateTimeDataScraped = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AvailableNetflix = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ContenttypeId = table.Column<int>(type: "int", nullable: false),
                    ContentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contentinformations", x => x.ContentinformationId);
                    table.ForeignKey(
                        name: "FK_Contentinformations_Contents_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Contents",
                        principalColumn: "ContentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contentinformations_Contenttypes_ContenttypeId",
                        column: x => x.ContenttypeId,
                        principalTable: "Contenttypes",
                        principalColumn: "ContenttypeId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ContentinformationsCountries",
                columns: table => new
                {
                    ContentinformationId = table.Column<int>(type: "int", nullable: false),
                    CountrieCountryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentinformationsCountries", x => new { x.ContentinformationId, x.CountrieCountryId });
                    table.ForeignKey(
                        name: "FK_ContentinformationsCountries_Contentinformations_Contentinfo~",
                        column: x => x.ContentinformationId,
                        principalTable: "Contentinformations",
                        principalColumn: "ContentinformationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContentinformationsCountries_Countries_CountrieCountryId",
                        column: x => x.CountrieCountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ContentinformationsGenres",
                columns: table => new
                {
                    ContentinformationId = table.Column<int>(type: "int", nullable: false),
                    GenreId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentinformationsGenres", x => new { x.ContentinformationId, x.GenreId });
                    table.ForeignKey(
                        name: "FK_ContentinformationsGenres_Contentinformations_Contentinforma~",
                        column: x => x.ContentinformationId,
                        principalTable: "Contentinformations",
                        principalColumn: "ContentinformationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContentinformationsGenres_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "GenreId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Contentinformations_ContentId",
                table: "Contentinformations",
                column: "ContentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contentinformations_ContenttypeId",
                table: "Contentinformations",
                column: "ContenttypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentinformationsCountries_CountrieCountryId",
                table: "ContentinformationsCountries",
                column: "CountrieCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentinformationsGenres_GenreId",
                table: "ContentinformationsGenres",
                column: "GenreId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentinformationsCountries");

            migrationBuilder.DropTable(
                name: "ContentinformationsGenres");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Contentinformations");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Contents");

            migrationBuilder.DropTable(
                name: "Contenttypes");
        }
    }
}
