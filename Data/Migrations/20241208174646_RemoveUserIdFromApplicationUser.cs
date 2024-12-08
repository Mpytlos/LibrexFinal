using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace librex3.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserIdFromApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReturned",
                table: "Books",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReturned",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Books");
        }
    }
}
