using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Catalog.API.Infrastructure.Migrations
{
    public partial class AddedMoreBookDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ISBN10",
                table: "CatalogItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ISBN13",
                table: "CatalogItems",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublicationDate",
                table: "CatalogItems",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TotalPage",
                table: "CatalogItems",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ISBN10",
                table: "CatalogItems");

            migrationBuilder.DropColumn(
                name: "ISBN13",
                table: "CatalogItems");

            migrationBuilder.DropColumn(
                name: "PublicationDate",
                table: "CatalogItems");

            migrationBuilder.DropColumn(
                name: "TotalPage",
                table: "CatalogItems");
        }
    }
}
