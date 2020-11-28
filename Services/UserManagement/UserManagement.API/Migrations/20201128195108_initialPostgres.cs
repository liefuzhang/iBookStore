using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace UserManagement.API.Migrations
{
    public partial class initialPostgres : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "NewsletterSubsriptions",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "NewsletterSubsriptions",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);
        }
    }
}
