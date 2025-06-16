using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EffortlessQA.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedreferenceOfReToTestSuite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequirementTestSuite",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequirementId = table.Column<Guid>(type: "uuid", nullable: false),
                    TestSuiteId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequirementTestSuite", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequirementTestSuite_Requirements_RequirementId",
                        column: x => x.RequirementId,
                        principalTable: "Requirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequirementTestSuite_TestSuites_TestSuiteId",
                        column: x => x.TestSuiteId,
                        principalTable: "TestSuites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequirementTestSuite_RequirementId",
                table: "RequirementTestSuite",
                column: "RequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_RequirementTestSuite_TestSuiteId",
                table: "RequirementTestSuite",
                column: "TestSuiteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequirementTestSuite");
        }
    }
}
