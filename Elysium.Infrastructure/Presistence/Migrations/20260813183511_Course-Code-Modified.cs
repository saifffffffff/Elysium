using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elysium.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CourseCodeModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Courses_Code",
                table: "Courses");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Courses_Code",
                table: "Courses");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Courses",
                type: "VARCHAR(6)",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(6)",
                oldMaxLength: 6);

            migrationBuilder.CreateCheckConstraint(
                name: "CK_Courses_Code",
                table: "Courses",
                sql: "LEN(TRIM(Code)) = 6");

            migrationBuilder.CreateCheckConstraint(
                name: "CK_Courses_Code_Chars",
                table: "Courses",
                sql: "Code NOT LIKE '%[^a-z0-9A-Z]%'");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_Code",
                table: "Courses",
                column: "Code",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Courses_Code",
                table: "Courses");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Courses_Code_Chars",
                table: "Courses");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Courses_Code",
                table: "Courses");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Courses",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(6)",
                oldMaxLength: 6);

            migrationBuilder.CreateCheckConstraint(
                name: "CK_Courses_Code",
                table: "Courses",
                sql: "LEN(TRIM(Code)) = 6");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_Code",
                table: "Courses",
                column: "Code",
                unique: true);
        }
    }
}
