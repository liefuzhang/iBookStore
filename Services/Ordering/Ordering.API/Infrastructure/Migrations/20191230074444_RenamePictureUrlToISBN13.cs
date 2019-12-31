using Microsoft.EntityFrameworkCore.Migrations;

namespace Ordering.API.Infrastructure.Migrations
{
    public partial class RenamePictureUrlToISBN13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PictureUrl",
                table: "OrderItems",
                newName: "ISBN13");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ISBN13",
                table: "OrderItems",
                newName: "PictureUrl");
        }
    }
}
