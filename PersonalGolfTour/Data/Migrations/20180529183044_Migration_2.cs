using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PersonalGolfTour.Data.Migrations
{
    public partial class Migration_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlacementRule",
                columns: table => new
                {
                    PlacementRuleId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Place = table.Column<int>(nullable: false),
                    Points = table.Column<int>(nullable: false),
                    TourId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlacementRule", x => x.PlacementRuleId);
                    table.ForeignKey(
                        name: "FK_PlacementRule_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TourResult",
                columns: table => new
                {
                    TourResultId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Place = table.Column<int>(nullable: false),
                    TourEventId = table.Column<int>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourResult", x => x.TourResultId);
                    table.ForeignKey(
                        name: "FK_TourResult_TourEvents_TourEventId",
                        column: x => x.TourEventId,
                        principalTable: "TourEvents",
                        principalColumn: "TourEventId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TourResult_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlacementRule_TourId",
                table: "PlacementRule",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourResult_TourEventId",
                table: "TourResult",
                column: "TourEventId");

            migrationBuilder.CreateIndex(
                name: "IX_TourResult_UserId",
                table: "TourResult",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlacementRule");

            migrationBuilder.DropTable(
                name: "TourResult");
        }
    }
}
