using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDMNG.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordFieldForTesting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PWString",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PWString",
                table: "AspNetUsers");
        }
    }
}
