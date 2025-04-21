using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PetApp.Migrations
{
    /// <inheritdoc />
    public partial class newData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "HealthRecords",
                columns: new[] { "Id", "Description", "EndDate", "PetId", "Price", "StartDate", "Type" },
                values: new object[,]
                {
                    { 1, "Szczepienie przeciwko wściekliźnie", null, 1, 50f, new DateTime(2023, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Szczepienie" },
                    { 2, "Badanie ogólne, profilaktyka", new DateTime(2023, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 100f, new DateTime(2023, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Wizyta u weterynarza" },
                    { 3, "Szczepienie przeciwko wirusowemu zapaleniu wątroby", null, 2, 45f, new DateTime(2023, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Szczepienie" },
                    { 4, "Kontrola stanu zdrowia, szczepienia", new DateTime(2023, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 80f, new DateTime(2023, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Wizyta u weterynarza" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "HealthRecords",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "HealthRecords",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "HealthRecords",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "HealthRecords",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
