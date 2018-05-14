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
                name: "Tours",
                columns: table => new
                {
                    TourId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Colour = table.Column<string>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    TourName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tours", x => x.TourId);
                });

            migrationBuilder.CreateTable(
                name: "TourEvents",
                columns: table => new
                {
                    TourEventId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    Location = table.Column<string>(nullable: true),
                    TourEventName = table.Column<string>(nullable: true),
                    TourId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourEvents", x => x.TourEventId);
                    table.ForeignKey(
                        name: "FK_TourEvents_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTour",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    TourId = table.Column<int>(nullable: false),
                    UserId1 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTour", x => new { x.UserId, x.TourId });
                    table.ForeignKey(
                        name: "FK_UserTour_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTour_AspNetUsers_UserId1",
                        column: x => x.UserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TourEvents_TourId",
                table: "TourEvents",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTour_TourId",
                table: "UserTour",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTour_UserId1",
                table: "UserTour",
                column: "UserId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TourEvents");

            migrationBuilder.DropTable(
                name: "UserTour");

            migrationBuilder.DropTable(
                name: "Tours");
        }
    }
}
