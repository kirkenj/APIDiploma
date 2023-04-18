using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class contractmodified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "IsConfirmed1",
                table: "Contracts");

            migrationBuilder.AddColumn<int>(
                name: "ConfirmedByUserID",
                table: "Contracts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ConfirmedByUserID",
                table: "Contracts",
                column: "ConfirmedByUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Contract_Confirmed_By_User",
                table: "Contracts",
                column: "ConfirmedByUserID",
                principalTable: "Users",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contract_Confirmed_By_User",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_ConfirmedByUserID",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "ConfirmedByUserID",
                table: "Contracts");

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "Contracts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed1",
                table: "Contracts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
