using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetApp.Migrations
{
    /// <inheritdoc />
    public partial class healthRecordDescNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Expenses_HealthRecordId",
                table: "Expenses");

            migrationBuilder.AlterColumn<float>(
                name: "Price",
                table: "HealthRecords",
                type: "real",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "HealthRecords",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_HealthRecordId",
                table: "Expenses",
                column: "HealthRecordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Expenses_HealthRecordId",
                table: "Expenses");

            migrationBuilder.AlterColumn<float>(
                name: "Price",
                table: "HealthRecords",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "HealthRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_HealthRecordId",
                table: "Expenses",
                column: "HealthRecordId",
                unique: true,
                filter: "[HealthRecordId] IS NOT NULL");
        }
    }
}
