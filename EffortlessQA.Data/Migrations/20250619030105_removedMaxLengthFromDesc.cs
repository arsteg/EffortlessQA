using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EffortlessQA.Data.Migrations
{
    /// <inheritdoc />
    public partial class removedMaxLengthFromDesc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequirementTestSuite_Requirements_RequirementId",
                table: "RequirementTestSuite");

            migrationBuilder.DropForeignKey(
                name: "FK_RequirementTestSuite_TestSuites_TestSuiteId",
                table: "RequirementTestSuite");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequirementTestSuite",
                table: "RequirementTestSuite");

            migrationBuilder.RenameTable(
                name: "RequirementTestSuite",
                newName: "RequirementTestSuites");

            migrationBuilder.RenameIndex(
                name: "IX_RequirementTestSuite_TestSuiteId",
                table: "RequirementTestSuites",
                newName: "IX_RequirementTestSuites_TestSuiteId");

            migrationBuilder.RenameIndex(
                name: "IX_RequirementTestSuite_RequirementId",
                table: "RequirementTestSuites",
                newName: "IX_RequirementTestSuites_RequirementId");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TestSuites",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TestRuns",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TestFolders",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TestCases",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Tenants",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Requirements",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Projects",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Permissions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Defects",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequirementTestSuites",
                table: "RequirementTestSuites",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementTestSuites_Requirements_RequirementId",
                table: "RequirementTestSuites",
                column: "RequirementId",
                principalTable: "Requirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementTestSuites_TestSuites_TestSuiteId",
                table: "RequirementTestSuites",
                column: "TestSuiteId",
                principalTable: "TestSuites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequirementTestSuites_Requirements_RequirementId",
                table: "RequirementTestSuites");

            migrationBuilder.DropForeignKey(
                name: "FK_RequirementTestSuites_TestSuites_TestSuiteId",
                table: "RequirementTestSuites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequirementTestSuites",
                table: "RequirementTestSuites");

            migrationBuilder.RenameTable(
                name: "RequirementTestSuites",
                newName: "RequirementTestSuite");

            migrationBuilder.RenameIndex(
                name: "IX_RequirementTestSuites_TestSuiteId",
                table: "RequirementTestSuite",
                newName: "IX_RequirementTestSuite_TestSuiteId");

            migrationBuilder.RenameIndex(
                name: "IX_RequirementTestSuites_RequirementId",
                table: "RequirementTestSuite",
                newName: "IX_RequirementTestSuite_RequirementId");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TestSuites",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TestRuns",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TestFolders",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TestCases",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Tenants",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Requirements",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Projects",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Permissions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Defects",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequirementTestSuite",
                table: "RequirementTestSuite",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementTestSuite_Requirements_RequirementId",
                table: "RequirementTestSuite",
                column: "RequirementId",
                principalTable: "Requirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementTestSuite_TestSuites_TestSuiteId",
                table: "RequirementTestSuite",
                column: "TestSuiteId",
                principalTable: "TestSuites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
