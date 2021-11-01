using Microsoft.EntityFrameworkCore.Migrations;

namespace DocumentManagementSystem.Migrations
{
    public partial class AddUserId_FK_UserId_Document : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_documents_users_uploadedby",
                table: "documents");

            migrationBuilder.DropIndex(
                name: "ix_documents_uploadedby",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "uploadedby",
                table: "documents");

            migrationBuilder.CreateIndex(
                name: "ix_documents_userid",
                table: "documents",
                column: "userid");

            migrationBuilder.AddForeignKey(
                name: "fk_documents_users_userid",
                table: "documents",
                column: "userid",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_documents_users_userid",
                table: "documents");

            migrationBuilder.DropIndex(
                name: "ix_documents_userid",
                table: "documents");

            migrationBuilder.AddColumn<string>(
                name: "uploadedby",
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
    }
}
