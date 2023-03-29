using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SoundpaysAdd.Data.Migrations
{
    public partial class addednewtableadd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Adds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CampaignId = table.Column<int>(type: "integer", nullable: false),
                    ShortName = table.Column<string>(type: "text", nullable: false),
                    LongName = table.Column<string>(type: "text", nullable: false),
                    AddType = table.Column<int>(type: "integer", nullable: false),
                    SoundCode = table.Column<string>(type: "text", nullable: false),
                    MinWidth = table.Column<decimal>(type: "numeric", nullable: false),
                    MaxWidth = table.Column<decimal>(type: "numeric", nullable: false),
                    MinHeight = table.Column<decimal>(type: "numeric", nullable: false),
                    MaxHeight = table.Column<decimal>(type: "numeric", nullable: false),
                    JsTag = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    StopDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StopTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    IsPaused = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Adds_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Adds_CampaignId",
                table: "Adds",
                column: "CampaignId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Adds");
        }
    }
}
