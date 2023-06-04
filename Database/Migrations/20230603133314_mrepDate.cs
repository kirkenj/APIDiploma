using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class mrepDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "MontYearAsDate",
                table: "MonthReports",
                type: "datetime(6)",
                nullable: false,
                computedColumnSql: "STR_TO_DATE(Concat('1',',', Month,',', Year),'%d,%m,%Y')",
                stored: false)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MontYearAsDate",
                table: "MonthReports");
        }
    }
}
