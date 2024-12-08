using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace librex3.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPenaltyFeeToLoan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOverdue",
                table: "Loans");

            migrationBuilder.AddColumn<decimal>(
                name: "PenaltyFee",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PenaltyFee",
                table: "Loans");

            migrationBuilder.AddColumn<bool>(
                name: "IsOverdue",
                table: "Loans",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
