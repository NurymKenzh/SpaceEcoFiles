using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SpaceEcoFiles.Migrations
{
    public partial class Doc_20201123_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Doc",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    Language = table.Column<int>(nullable: false),
                    DocTypeId = table.Column<int>(nullable: false),
                    DocFormatId = table.Column<int>(nullable: false),
                    File = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doc", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Doc_DocFormat_DocFormatId",
                        column: x => x.DocFormatId,
                        principalTable: "DocFormat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Doc_DocType_DocTypeId",
                        column: x => x.DocTypeId,
                        principalTable: "DocType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Doc_DocFormatId",
                table: "Doc",
                column: "DocFormatId");

            migrationBuilder.CreateIndex(
                name: "IX_Doc_DocTypeId",
                table: "Doc",
                column: "DocTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Doc");
        }
    }
}
