using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecretShare.Migrations
{
    /// <inheritdoc />
    public partial class DeleteFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasbeenDowloaded",
                table: "UploadedTexts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasbeenDowloaded",
                table: "UploadedFiles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasbeenDowloaded",
                table: "UploadedTexts");

            migrationBuilder.DropColumn(
                name: "HasbeenDowloaded",
                table: "UploadedFiles");
        }
    }
}
