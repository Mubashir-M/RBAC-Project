using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RBacServer.Migrations
{
    /// <inheritdoc />
    public partial class ReAddSourceIPToEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SourceIPAddress",
                table: "Events",
                type: "TEXT",
                maxLength: 45,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceIPAddress",
                table: "Events");
        }
    }
}
