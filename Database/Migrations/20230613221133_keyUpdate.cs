using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class keyUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Contracts_ContractIdentifier",
                table: "Contracts");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ContractIdentifier_ConclusionDate",
                table: "Contracts",
                columns: new[] { "ContractIdentifier", "ConclusionDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Contracts_ContractIdentifier_ConclusionDate",
                table: "Contracts");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ContractIdentifier",
                table: "Contracts",
                column: "ContractIdentifier",
                unique: true);
        }
    }
}
