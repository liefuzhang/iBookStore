using Microsoft.EntityFrameworkCore.Migrations;

namespace Catalog.API.Infrastructure.Migrations
{
    public partial class RenamePictureUrlToISBN13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PictureUrl",
                table: "CatalogItems");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PictureUrl",
                table: "CatalogItems",
                nullable: true);
        }
    }
}
