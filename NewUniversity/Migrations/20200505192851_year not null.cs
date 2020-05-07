using Microsoft.EntityFrameworkCore.Migrations;

namespace NewUniversity.Migrations
{
    public partial class yearnotnull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Year",
                table: "Enrollment",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Year",
                table: "Enrollment",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
