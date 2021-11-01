using Microsoft.EntityFrameworkCore.Migrations;

namespace DocumentManagementSystem.Migrations
{
    public partial class UpdateDocument_Department : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "department",
                table: "documents",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "department",
                table: "documents");
        }
    }
}
