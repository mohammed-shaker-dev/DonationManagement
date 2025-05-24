using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class project2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Projects",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ProjectType",
                table: "Projects",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<int>(
                name: "ProjectType",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 2,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);
        }
    }
}
