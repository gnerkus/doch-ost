using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerRelationToDocumentInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "DocumentInfos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentInfos_OwnerId",
                table: "DocumentInfos",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentInfos_AspNetUsers_OwnerId",
                table: "DocumentInfos",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentInfos_AspNetUsers_OwnerId",
                table: "DocumentInfos");

            migrationBuilder.DropIndex(
                name: "IX_DocumentInfos_OwnerId",
                table: "DocumentInfos");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "DocumentInfos");
        }
    }
}
