using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetApp.Migrations
{
    /// <inheritdoc />
    public partial class data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Name", "Password" },
                values: new object[] { 1, "test@test.com", "Marta", "1234" });

            migrationBuilder.InsertData(
                table: "Pets",
                columns: new[] { "Id", "BirthDate", "Breed", "Name", "Species", "UserId", "Weight" },
                values: new object[] { 1, new DateTime(2020, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Labrador", "Luna", "Pies", 1, 25.5f });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
