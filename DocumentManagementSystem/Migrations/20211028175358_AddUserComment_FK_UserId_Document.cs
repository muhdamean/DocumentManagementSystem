using Microsoft.EntityFrameworkCore.Migrations;

namespace DocumentManagementSystem.Migrations
{
    public partial class AddUserComment_FK_UserId_Document : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "uploadcomment",
                table: "documents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "userid",
                table: "documents",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_documents_uploadedby",
                table: "documents",
                column: "uploadedby");

            migrationBuilder.AddForeignKey(
                name: "fk_documents_users_uploadedby",
                table: "documents",
                column: "uploadedby",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_documents_users_uploadedby",
                table: "documents");

            migrationBuilder.DropIndex(
                name: "ix_documents_uploadedby",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "uploadcomment",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "userid",
                table: "documents");
        }
    }
}
