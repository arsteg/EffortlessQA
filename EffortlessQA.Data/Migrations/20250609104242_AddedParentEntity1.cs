using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EffortlessQA.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedParentEntity1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActualResult",
                table: "TestCases",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "TestCases",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Precondition",
                table: "TestCases",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Screenshot",
                table: "TestCases",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "TestCases",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TestData",
                table: "TestCases",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualResult",
                table: "TestCases");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "TestCases");

            migrationBuilder.DropColumn(
                name: "Precondition",
                table: "TestCases");

            migrationBuilder.DropColumn(
                name: "Screenshot",
                table: "TestCases");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TestCases");

            migrationBuilder.DropColumn(
                name: "TestData",
                table: "TestCases");
        }
    }
}
