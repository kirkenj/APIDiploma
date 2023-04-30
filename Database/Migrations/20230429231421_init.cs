using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Surname = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Patronymic = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    Login = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "NVARCHAR(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Roles_Users",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    DepartmentID = table.Column<int>(type: "int", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ConfirmedByUserID = table.Column<int>(type: "int", nullable: true),
                    ParentContractID = table.Column<int>(type: "int", nullable: true),
                    LectionsMaxTime = table.Column<int>(type: "int", nullable: false),
                    PracticalClassesMaxTime = table.Column<int>(type: "int", nullable: false),
                    LaboratoryClassesMaxTime = table.Column<int>(type: "int", nullable: false),
                    ConsultationsMaxTime = table.Column<int>(type: "int", nullable: false),
                    OtherTeachingClassesMaxTime = table.Column<int>(type: "int", nullable: false),
                    CreditsMaxTime = table.Column<int>(type: "int", nullable: false),
                    ExamsMaxTime = table.Column<int>(type: "int", nullable: false),
                    CourseProjectsMaxTime = table.Column<int>(type: "int", nullable: false),
                    InterviewsMaxTime = table.Column<int>(type: "int", nullable: false),
                    TestsAndReferatsMaxTime = table.Column<int>(type: "int", nullable: false),
                    InternshipsMaxTime = table.Column<int>(type: "int", nullable: false),
                    DiplomasMaxTime = table.Column<int>(type: "int", nullable: false),
                    DiplomasReviewsMaxTime = table.Column<int>(type: "int", nullable: false),
                    SECMaxTime = table.Column<int>(type: "int", nullable: false),
                    GraduatesManagementMaxTime = table.Column<int>(type: "int", nullable: false),
                    GraduatesAcademicWorkMaxTime = table.Column<int>(type: "int", nullable: false),
                    PlasticPosesDemonstrationMaxTime = table.Column<int>(type: "int", nullable: false),
                    TestingEscortMaxTime = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Contract_Confirmed_By_User",
                        column: x => x.ConfirmedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Contracts_Departments",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contracts_Users",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contrcat_Contract",
                        column: x => x.ParentContractID,
                        principalTable: "Contracts",
                        principalColumn: "ID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MonthReports",
                columns: table => new
                {
                    ContractID = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    LectionsTime = table.Column<int>(type: "int", nullable: false),
                    PracticalClassesTime = table.Column<int>(type: "int", nullable: false),
                    LaboratoryClassesTime = table.Column<int>(type: "int", nullable: false),
                    ConsultationsTime = table.Column<int>(type: "int", nullable: false),
                    OtherTeachingClassesTime = table.Column<int>(type: "int", nullable: false),
                    CreditsTime = table.Column<int>(type: "int", nullable: false),
                    ExamsTime = table.Column<int>(type: "int", nullable: false),
                    CourseProjectsTime = table.Column<int>(type: "int", nullable: false),
                    InterviewsTime = table.Column<int>(type: "int", nullable: false),
                    TestsAndReferatsTime = table.Column<int>(type: "int", nullable: false),
                    InternshipsTime = table.Column<int>(type: "int", nullable: false),
                    DiplomasTime = table.Column<int>(type: "int", nullable: false),
                    DiplomasReviewsTime = table.Column<int>(type: "int", nullable: false),
                    SECTime = table.Column<int>(type: "int", nullable: false),
                    GraduatesManagementTime = table.Column<int>(type: "int", nullable: false),
                    GraduatesAcademicWorkTime = table.Column<int>(type: "int", nullable: false),
                    PlasticPosesDemonstrationTime = table.Column<int>(type: "int", nullable: false),
                    TestingEscortTime = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthReports", x => new { x.Month, x.Year, x.ContractID });
                    table.ForeignKey(
                        name: "FK_MonthReports_Contracts_ContractID",
                        column: x => x.ContractID,
                        principalTable: "Contracts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "ID", "Name" },
                values: new object[] { 1, "SuperAdmin" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "ID", "Name" },
                values: new object[] { 2, "User" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "ID", "Name" },
                values: new object[] { 3, "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ConfirmedByUserID",
                table: "Contracts",
                column: "ConfirmedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_DepartmentID",
                table: "Contracts",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ParentContractID",
                table: "Contracts",
                column: "ParentContractID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_UserID",
                table: "Contracts",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                table: "Departments",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MonthReports_ContractID",
                table: "MonthReports",
                column: "ContractID");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Login",
                table: "Users",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleID",
                table: "Users",
                column: "RoleID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonthReports");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
