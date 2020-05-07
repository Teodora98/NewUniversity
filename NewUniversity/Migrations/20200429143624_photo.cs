using Microsoft.EntityFrameworkCore.Migrations;

namespace NewUniversity.Migrations
{
    public partial class photo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TeacherProfileImage",
                table: "Teacher",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserProfileImage",
                table: "Student",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeacherProfileImage",
                table: "Teacher");

            migrationBuilder.DropColumn(
                name: "UserProfileImage",
                table: "Student");
        }
    }
}
