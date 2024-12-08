using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace librex3.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Books");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
