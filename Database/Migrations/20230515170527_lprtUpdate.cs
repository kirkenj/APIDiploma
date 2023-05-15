using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class lprtUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SourceContractID",
                table: "ContractLinkingParts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ContractLinkingParts_SourceContractID",
                table: "ContractLinkingParts",
                column: "SourceContractID");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractLinkingParts_Contracts_SourceContractID",
                table: "ContractLinkingParts",
                column: "SourceContractID",
                principalTable: "Contracts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractLinkingParts_Contracts_SourceContractID",
                table: "ContractLinkingParts");

            migrationBuilder.DropIndex(
                name: "IX_ContractLinkingParts_SourceContractID",
                table: "ContractLinkingParts");

            migrationBuilder.DropColumn(
                name: "SourceContractID",
                table: "ContractLinkingParts");
        }
    }
}
