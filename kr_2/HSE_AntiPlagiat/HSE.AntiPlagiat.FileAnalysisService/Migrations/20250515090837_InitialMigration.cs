using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HSE.AntiPlagiat.FileAnalysisService.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnalysisResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParagraphCount = table.Column<int>(type: "integer", nullable: false),
                    WordCount = table.Column<int>(type: "integer", nullable: false),
                    CharacterCount = table.Column<int>(type: "integer", nullable: false),
                    WordCloudUrl = table.Column<string>(type: "text", nullable: false),
                    AnalysisDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SimilarityResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalFileId = table.Column<Guid>(type: "uuid", nullable: false),
                    ComparedFileId = table.Column<Guid>(type: "uuid", nullable: false),
                    SimilarityPercentage = table.Column<double>(type: "double precision", nullable: false),
                    ComparisonDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimilarityResults", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalysisResults");

            migrationBuilder.DropTable(
                name: "SimilarityResults");
        }
    }
}
