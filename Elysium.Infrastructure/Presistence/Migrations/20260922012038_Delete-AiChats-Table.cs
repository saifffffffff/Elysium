using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elysium.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleteAiChatsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AiChatMessages_AiChats_AiChatId",
                table: "AiChatMessages");

            migrationBuilder.DropTable(
                name: "AiChats");

            migrationBuilder.RenameColumn(
                name: "AiChatId",
                table: "AiChatMessages",
                newName: "StudentSessionId");

            migrationBuilder.RenameIndex(
                name: "IX_AiChatMessages_AiChatId",
                table: "AiChatMessages",
                newName: "IX_AiChatMessages_StudentSessionId");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Enrollments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "AskedAt",
                table: "AiChatMessages",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddCheckConstraint(
                name: "CK_AiChatMessages_Dates",
                table: "AiChatMessages",
                sql: "(AnsweredAt IS NULL OR AnsweredAt > AskedAt)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_AiChatMessages_Question",
                table: "AiChatMessages",
                sql: "LEN(TRIM(Question)) > 0");

            migrationBuilder.AddForeignKey(
                name: "FK_AiChatMessages_StudentSessions_StudentSessionId",
                table: "AiChatMessages",
                column: "StudentSessionId",
                principalTable: "StudentSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AiChatMessages_StudentSessions_StudentSessionId",
                table: "AiChatMessages");

            migrationBuilder.DropCheckConstraint(
                name: "CK_AiChatMessages_Dates",
                table: "AiChatMessages");

            migrationBuilder.DropCheckConstraint(
                name: "CK_AiChatMessages_Question",
                table: "AiChatMessages");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Enrollments");

            migrationBuilder.RenameColumn(
                name: "StudentSessionId",
                table: "AiChatMessages",
                newName: "AiChatId");

            migrationBuilder.RenameIndex(
                name: "IX_AiChatMessages_StudentSessionId",
                table: "AiChatMessages",
                newName: "IX_AiChatMessages_AiChatId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AskedAt",
                table: "AiChatMessages",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.CreateTable(
                name: "AiChats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentSessionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiChats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AiChats_StudentSessions_StudentSessionId",
                        column: x => x.StudentSessionId,
                        principalTable: "StudentSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AiChats_StudentSessionId",
                table: "AiChats",
                column: "StudentSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AiChatMessages_AiChats_AiChatId",
                table: "AiChatMessages",
                column: "AiChatId",
                principalTable: "AiChats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
