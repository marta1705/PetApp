using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetApp.Migrations
{
    /// <inheritdoc />
    public partial class data2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Pets",
                columns: new[] { "Id", "BirthDate", "Breed", "Name", "PhotoUrl", "Species", "UserId", "Weight" },
                values: new object[] { 2, new DateTime(2022, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bengalski", "Ed", "https://images.unsplash.com/photo-1505015390928-f9e55218544f?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MjR8fGJlbmdhbCUyMGNhdHxlbnwwfHwwfHx8MA%3D%3D", "Kot", 1, 4.2f });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
