using Microsoft.EntityFrameworkCore.Migrations;

namespace Ordering.API.Infrastructure.Migrations
{
    public partial class AddedCurrencyAndCurrencyRateToOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Orders",
                nullable: true,
                defaultValue: "NZD");

            migrationBuilder.AddColumn<decimal>(
                name: "CurrencyRate",
                table: "Orders",
                nullable: false,
                defaultValue: 1m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CurrencyRate",
                table: "Orders");
        }
    }
}
