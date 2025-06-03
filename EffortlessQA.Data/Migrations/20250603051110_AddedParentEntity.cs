using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EffortlessQA.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedParentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentRequirementId",
                table: "Requirements",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requirements_ParentRequirementId",
                table: "Requirements",
                column: "ParentRequirementId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requirements_Requirements_ParentRequirementId",
                table: "Requirements",
                column: "ParentRequirementId",
                principalTable: "Requirements",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requirements_Requirements_ParentRequirementId",
                table: "Requirements");

            migrationBuilder.DropIndex(
                name: "IX_Requirements_ParentRequirementId",
                table: "Requirements");

            migrationBuilder.DropColumn(
                name: "ParentRequirementId",
                table: "Requirements");
        }
    }
}
