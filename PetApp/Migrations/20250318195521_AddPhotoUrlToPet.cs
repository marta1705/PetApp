using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetApp.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoUrlToPet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "Pets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 1,
                column: "PhotoUrl",
                value: "https://unsplash.com/photos/white-dog-on-sands-between-leafed-plants-O6UzDPFH36g");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "Pets");
        }
    }
}
