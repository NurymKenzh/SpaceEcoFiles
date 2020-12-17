using Microsoft.EntityFrameworkCore.Migrations;

namespace SpaceEcoFiles.Migrations
{
    public partial class Doc_20201217_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DownloadsCount",
                table: "Doc",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DownloadsCount",
                table: "Doc");
        }
    }
}
