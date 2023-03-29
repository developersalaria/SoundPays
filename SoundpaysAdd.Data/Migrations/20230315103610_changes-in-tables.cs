using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoundpaysAdd.Data.Migrations
{
    public partial class changesintables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "IsPaused",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "IsPaused",
                table: "Attachments");

            migrationBuilder.RenameColumn(
                name: "minimpressions",
                table: "Campaigns",
                newName: "MinImpressions");

            migrationBuilder.RenameColumn(
                name: "maximpressions",
                table: "Campaigns",
                newName: "MaxImpressions");

            migrationBuilder.RenameColumn(
                name: "JsTag",
                table: "Adds",
                newName: "JSTag");

            migrationBuilder.AddColumn<bool>(
                name: "IsPaused",
                table: "Devices",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StopDate",
                table: "Campaigns",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Campaigns",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaused",
                table: "Devices");

            migrationBuilder.RenameColumn(
                name: "MinImpressions",
                table: "Campaigns",
                newName: "minimpressions");

            migrationBuilder.RenameColumn(
                name: "MaxImpressions",
                table: "Campaigns",
                newName: "maximpressions");

            migrationBuilder.RenameColumn(
                name: "JSTag",
                table: "Adds",
                newName: "JsTag");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Events",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsPaused",
                table: "Events",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Events",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StopDate",
                table: "Campaigns",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Campaigns",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<bool>(
                name: "IsPaused",
                table: "Attachments",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
