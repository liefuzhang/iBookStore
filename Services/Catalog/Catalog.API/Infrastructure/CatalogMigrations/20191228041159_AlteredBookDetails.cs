using Microsoft.EntityFrameworkCore.Migrations;

namespace Catalog.API.Infrastructure.Migrations
{
    public partial class AlteredBookDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ISBN10",
                table: "CatalogItems");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CatalogItems",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CatalogItems",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 200);

            migrationBuilder.AddColumn<string>(
                name: "ISBN10",
                table: "CatalogItems",
                nullable: true);
        }
    }
}
