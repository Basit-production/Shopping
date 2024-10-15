using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ahmed_mart.Migrations
{
    /// <inheritdoc />
    public partial class StoreRemove : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Template",
                table: "Store");

            migrationBuilder.DropColumn(
                name: "TemplatePath",
                table: "Store");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Template",
                table: "Store",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TemplatePath",
                table: "Store",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
