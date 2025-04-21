using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetApp.Migrations
{
    /// <inheritdoc />
    public partial class data1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 1,
                column: "PhotoUrl",
                value: "https://images.unsplash.com/photo-1529831129093-0fa4866281ee?q=80&w=1974&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 1,
                column: "PhotoUrl",
                value: "https://unsplash.com/photos/white-dog-on-sands-between-leafed-plants-O6UzDPFH36g");
        }
    }
}
