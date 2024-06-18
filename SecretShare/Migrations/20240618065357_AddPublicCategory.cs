using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecretShare.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PublicText",
                table: "UploadedTexts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PublicFile",
                table: "UploadedFiles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicText",
                table: "UploadedTexts");

            migrationBuilder.DropColumn(
                name: "PublicFile",
                table: "UploadedFiles");
        }
    }
}
