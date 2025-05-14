using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixmoneyobject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Expenses",
                newName: "Amount_Amount");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "Expenses");

            migrationBuilder.RenameColumn(
                name: "Amount_Amount",
                table: "Expenses",
                newName: "Value");
        }
    }
}
