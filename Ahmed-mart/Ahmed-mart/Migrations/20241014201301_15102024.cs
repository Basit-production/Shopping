using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ahmed_mart.Migrations
{
    /// <inheritdoc />
    public partial class _15102024 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Admin",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenCreatedAt",
                table: "Admin",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenTokenExpiresAt",
                table: "Admin",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Admin");

            migrationBuilder.DropColumn(
                name: "RefreshTokenCreatedAt",
                table: "Admin");

            migrationBuilder.DropColumn(
                name: "RefreshTokenTokenExpiresAt",
                table: "Admin");
        }
    }
}
