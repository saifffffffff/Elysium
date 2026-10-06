using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elysium.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsInSessionFlagToSessionStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInSession",
                table: "StudentSessions",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInSession",
                table: "StudentSessions");
        }
    }
}
