using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AcademicDegrees",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicDegrees", x => x.ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ContractTypes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractTypes", x => x.ID);
                })
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
                name: "AcademicDegreePriceAssignations",
                columns: table => new
                {
                    AssignationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ObjectIdentifier = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicDegreePriceAssignations", x => new { x.AssignationDate, x.ObjectIdentifier });
                    table.ForeignKey(
                        name: "FK_AcademicDegree_AcademicDegreeValueAssignation",
                        column: x => x.ObjectIdentifier,
                        principalTable: "AcademicDegrees",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ContractTypePriceAssignments",
                columns: table => new
                {
                    AssignationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ObjectIdentifier = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractTypePriceAssignments", x => new { x.AssignationDate, x.ObjectIdentifier });
                    table.ForeignKey(
                        name: "FK_ContractType_ContractTypeValueAssignation",
                        column: x => x.ObjectIdentifier,
                        principalTable: "ContractTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
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
                    ContractTypeID = table.Column<int>(type: "int", nullable: false),
                    ContractIdentifier = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PeriodStart = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ParentContractID = table.Column<int>(type: "int", nullable: true),
                    LectionsMaxTime = table.Column<double>(type: "double", nullable: false),
                    PracticalClassesMaxTime = table.Column<double>(type: "double", nullable: false),
                    LaboratoryClassesMaxTime = table.Column<double>(type: "double", nullable: false),
                    ConsultationsMaxTime = table.Column<double>(type: "double", nullable: false),
                    OtherTeachingClassesMaxTime = table.Column<double>(type: "double", nullable: false),
                    CreditsMaxTime = table.Column<double>(type: "double", nullable: false),
                    ExamsMaxTime = table.Column<double>(type: "double", nullable: false),
                    CourseProjectsMaxTime = table.Column<double>(type: "double", nullable: false),
                    InterviewsMaxTime = table.Column<double>(type: "double", nullable: false),
                    TestsAndReferatsMaxTime = table.Column<double>(type: "double", nullable: false),
                    InternshipsMaxTime = table.Column<double>(type: "double", nullable: false),
                    DiplomasMaxTime = table.Column<double>(type: "double", nullable: false),
                    DiplomasReviewsMaxTime = table.Column<double>(type: "double", nullable: false),
                    SECMaxTime = table.Column<double>(type: "double", nullable: false),
                    GraduatesManagementMaxTime = table.Column<double>(type: "double", nullable: false),
                    GraduatesAcademicWorkMaxTime = table.Column<double>(type: "double", nullable: false),
                    PlasticPosesDemonstrationMaxTime = table.Column<double>(type: "double", nullable: false),
                    TestingEscortMaxTime = table.Column<double>(type: "double", nullable: false),
                    ConfirmedByUserID = table.Column<int>(type: "int", nullable: true)
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
                        name: "FK_Contract_ContractType",
                        column: x => x.ContractTypeID,
                        principalTable: "ContractTypes",
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
                name: "UserAcademicDegreeAssignaments",
                columns: table => new
                {
                    AssignationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ObjectIdentifier = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAcademicDegreeAssignaments", x => new { x.AssignationDate, x.ObjectIdentifier });
                    table.ForeignKey(
                        name: "FK_UserAcademicDegreeAssignaments_AcademicDegrees_Value",
                        column: x => x.Value,
                        principalTable: "AcademicDegrees",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_UserAcademicDegreeAssignation",
                        column: x => x.ObjectIdentifier,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MonthReports",
                columns: table => new
                {
                    ContractID = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    LectionsTime = table.Column<double>(type: "double", nullable: false),
                    PracticalClassesTime = table.Column<double>(type: "double", nullable: false),
                    LaboratoryClassesTime = table.Column<double>(type: "double", nullable: false),
                    ConsultationsTime = table.Column<double>(type: "double", nullable: false),
                    OtherTeachingClassesTime = table.Column<double>(type: "double", nullable: false),
                    CreditsTime = table.Column<double>(type: "double", nullable: false),
                    ExamsTime = table.Column<double>(type: "double", nullable: false),
                    CourseProjectsTime = table.Column<double>(type: "double", nullable: false),
                    InterviewsTime = table.Column<double>(type: "double", nullable: false),
                    TestsAndReferatsTime = table.Column<double>(type: "double", nullable: false),
                    InternshipsTime = table.Column<double>(type: "double", nullable: false),
                    DiplomasTime = table.Column<double>(type: "double", nullable: false),
                    DiplomasReviewsTime = table.Column<double>(type: "double", nullable: false),
                    SECTime = table.Column<double>(type: "double", nullable: false),
                    GraduatesManagementTime = table.Column<double>(type: "double", nullable: false),
                    GraduatesAcademicWorkTime = table.Column<double>(type: "double", nullable: false),
                    PlasticPosesDemonstrationTime = table.Column<double>(type: "double", nullable: false),
                    TestingEscortTime = table.Column<double>(type: "double", nullable: false),
                    BlockedByUserID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthReports", x => new { x.Month, x.Year, x.ContractID });
                    table.ForeignKey(
                        name: "FK_MonthReport_Blocked_By_User",
                        column: x => x.BlockedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_MonthReports_Contracts_ContractID",
                        column: x => x.ContractID,
                        principalTable: "Contracts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AcademicDegrees",
                columns: new[] { "ID", "Name" },
                values: new object[,]
                {
                    { 1, "Doctor" },
                    { 2, "Professor" }
                });

            migrationBuilder.InsertData(
                table: "ContractTypes",
                columns: new[] { "ID", "Name" },
                values: new object[,]
                {
                    { 1, "Ordinary" },
                    { 2, "Advanced" }
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "ID", "Name" },
                values: new object[,]
                {
                    { 1, "FITR" },
                    { 2, "FTUG" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "ID", "Name" },
                values: new object[,]
                {
                    { 1, "SuperAdmin" },
                    { 2, "User" },
                    { 3, "Admin" }
                });

            migrationBuilder.InsertData(
                table: "AcademicDegreePriceAssignations",
                columns: new[] { "AssignationDate", "ObjectIdentifier", "Value" },
                values: new object[,]
                {
                    { new DateTime(2023, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 12.0 },
                    { new DateTime(2023, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 10.0 }
                });

            migrationBuilder.InsertData(
                table: "ContractTypePriceAssignments",
                columns: new[] { "AssignationDate", "ObjectIdentifier", "Value" },
                values: new object[,]
                {
                    { new DateTime(2023, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 12.0 },
                    { new DateTime(2023, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 10.0 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "ID", "Login", "Name", "PasswordHash", "Patronymic", "RoleID", "Surname" },
                values: new object[] { 1, "admin", "admin", "!#/)zW��C�JJ��", "admin", 1, "admin" });

            migrationBuilder.InsertData(
                table: "UserAcademicDegreeAssignaments",
                columns: new[] { "AssignationDate", "ObjectIdentifier", "Value" },
                values: new object[] { new DateTime(2023, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicDegreePriceAssignations_ObjectIdentifier",
                table: "AcademicDegreePriceAssignations",
                column: "ObjectIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicDegrees_Name",
                table: "AcademicDegrees",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ConfirmedByUserID",
                table: "Contracts",
                column: "ConfirmedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ContractIdentifier",
                table: "Contracts",
                column: "ContractIdentifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ContractTypeID",
                table: "Contracts",
                column: "ContractTypeID");

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
                name: "IX_ContractTypePriceAssignments_ObjectIdentifier",
                table: "ContractTypePriceAssignments",
                column: "ObjectIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_ContractTypes_Name",
                table: "ContractTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                table: "Departments",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MonthReports_BlockedByUserID",
                table: "MonthReports",
                column: "BlockedByUserID");

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
                name: "IX_UserAcademicDegreeAssignaments_ObjectIdentifier",
                table: "UserAcademicDegreeAssignaments",
                column: "ObjectIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_UserAcademicDegreeAssignaments_Value",
                table: "UserAcademicDegreeAssignaments",
                column: "Value");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcademicDegreePriceAssignations");

            migrationBuilder.DropTable(
                name: "ContractTypePriceAssignments");

            migrationBuilder.DropTable(
                name: "MonthReports");

            migrationBuilder.DropTable(
                name: "UserAcademicDegreeAssignaments");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "AcademicDegrees");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "ContractTypes");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
