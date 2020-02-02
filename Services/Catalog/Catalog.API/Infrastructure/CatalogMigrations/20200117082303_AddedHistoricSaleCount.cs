using Microsoft.EntityFrameworkCore.Migrations;

namespace Catalog.API.Infrastructure.Migrations
{
    public partial class AddedHistoricSaleCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HistoricSaleCount",
                table: "CatalogItems",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HistoricSaleCount",
                table: "CatalogItems");
        }
    }
}
