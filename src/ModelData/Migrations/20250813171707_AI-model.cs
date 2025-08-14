using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModelData.Migrations
{
    /// <inheritdoc />
    public partial class AImodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GeminiApiKey",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeminiApiKey",
                table: "Settings");
        }
    }
}
