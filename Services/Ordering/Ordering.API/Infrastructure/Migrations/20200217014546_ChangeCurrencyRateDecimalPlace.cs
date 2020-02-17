using Microsoft.EntityFrameworkCore.Migrations;

namespace Ordering.API.Infrastructure.Migrations
{
    public partial class ChangeCurrencyRateDecimalPlace : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "CurrencyRate",
                table: "Orders",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 1m,
                oldClrType: typeof(decimal),
                oldDefaultValue: 1m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "CurrencyRate",
                table: "Orders",
                nullable: false,
                defaultValue: 1m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)",
                oldDefaultValue: 1m);
        }
    }
}
