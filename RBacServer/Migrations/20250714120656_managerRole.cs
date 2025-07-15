using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RBacServer.Migrations
{
    /// <inheritdoc />
    public partial class managerRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "EditUser");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "name" },
                values: new object[] { 3, "Manager with access to users in a project", "Manager" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "EdiUser");
        }
    }
}
