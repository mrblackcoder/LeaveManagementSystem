using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveManagementSystemNew.Migrations
{
    /// <inheritdoc />
    public partial class AM2024 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "accounts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "accounts");
        }
    }
}
