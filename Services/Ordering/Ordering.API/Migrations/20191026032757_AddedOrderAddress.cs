using Microsoft.EntityFrameworkCore.Migrations;

namespace Ordering.API.Migrations
{
    public partial class AddedOrderAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDraft",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "ZipCode",
                table: "Orders",
                newName: "Address_ZipCode");

            migrationBuilder.RenameColumn(
                name: "Street",
                table: "Orders",
                newName: "Address_Street");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "Orders",
                newName: "Address_State");

            migrationBuilder.RenameColumn(
                name: "Country",
                table: "Orders",
                newName: "Address_Country");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "Orders",
                newName: "Address_City");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address_ZipCode",
                table: "Orders",
                newName: "ZipCode");

            migrationBuilder.RenameColumn(
                name: "Address_Street",
                table: "Orders",
                newName: "Street");

            migrationBuilder.RenameColumn(
                name: "Address_State",
                table: "Orders",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "Address_Country",
                table: "Orders",
                newName: "Country");

            migrationBuilder.RenameColumn(
                name: "Address_City",
                table: "Orders",
                newName: "City");

            migrationBuilder.AddColumn<bool>(
                name: "IsDraft",
                table: "Orders",
                nullable: false,
                defaultValue: false);
        }
    }
}
