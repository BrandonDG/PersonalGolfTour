using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PersonalGolfTour.Data.Migrations
{
    public partial class Migration_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlacementRule_Tours_TourId",
                table: "PlacementRule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlacementRule",
                table: "PlacementRule");

            migrationBuilder.RenameTable(
                name: "PlacementRule",
                newName: "PlacementRules");

            migrationBuilder.RenameIndex(
                name: "IX_PlacementRule_TourId",
                table: "PlacementRules",
                newName: "IX_PlacementRules_TourId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlacementRules",
                table: "PlacementRules",
                column: "PlacementRuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlacementRules_Tours_TourId",
                table: "PlacementRules",
                column: "TourId",
                principalTable: "Tours",
                principalColumn: "TourId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlacementRules_Tours_TourId",
                table: "PlacementRules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlacementRules",
                table: "PlacementRules");

            migrationBuilder.RenameTable(
                name: "PlacementRules",
                newName: "PlacementRule");

            migrationBuilder.RenameIndex(
                name: "IX_PlacementRules_TourId",
                table: "PlacementRule",
                newName: "IX_PlacementRule_TourId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlacementRule",
                table: "PlacementRule",
                column: "PlacementRuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlacementRule_Tours_TourId",
                table: "PlacementRule",
                column: "TourId",
                principalTable: "Tours",
                principalColumn: "TourId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
