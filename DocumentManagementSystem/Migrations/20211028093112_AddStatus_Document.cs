using Microsoft.EntityFrameworkCore.Migrations;

namespace DocumentManagementSystem.Migrations
{
    public partial class AddStatus_Document : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "documents",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "documents");
        }
    }
}
