﻿// <auto-generated />
using System;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Database.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230415133011_contract_modified")]
    partial class contractmodified
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Database.Entities.Contract", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int?>("ConfirmedByUserID")
                        .HasColumnType("int")
                        .HasColumnName("ConfirmedByUserID");

                    b.Property<int>("ConsultationsMaxTime")
                        .HasColumnType("int")
                        .HasColumnName("ConsultationsMaxTime");

                    b.Property<int>("CourseProjectsMaxTime")
                        .HasColumnType("int")
                        .HasColumnName("CourseProjectsMaxTime");

                    b.Property<int>("CreditsMaxTime")
                        .HasColumnType("int")
                        .HasColumnName("CreditsMaxTime");

                    b.Property<int>("DepartmentID")
                        .HasColumnType("int");

                    b.Property<int>("DiplomasMaxTime")
                        .HasColumnType("int")
                        .HasColumnName("DiplomasMaxTime");

                    b.Property<int>("DiplomasReviewsMaxTime")
                        .HasColumnType("int")
                        .HasColumnName("DiplomasReviewsMaxTime");

                    b.Property<int>("ExamsMaxTime")
                        .HasColumnType("int")
                        .HasColumnName("ExamsMaxTime");

                    b.Property<int>("GraduatesAcademicWorkMaxTime")
                        .HasColumnType("int")
                        .HasColumnName("GraduatesAcademicWorkMaxTime");

                    b.Property<int>("GraduatesManagementMaxTime")
                        .HasColumnType("int")
                        .HasColumnName("GraduatesManagementMaxTime");

                    b.Property<int>("InternshipsMaxTime")
                        .HasColumnType("int")
                        .HasColumnName("InternshipsMaxTime");

                    b.Property<int>("InterviewsMaxTime")
                        .HasColumnType("int")
                        .HasColumnName("InterviewsMaxTime");

                    b.Property<int>("LaboratoryClassesMaxTime")
                        .HasColumnType("int")
                        .HasColumnName("LaboratoryClassesMaxTime");

                    b.Property<int>("LectionsMaxTime")
                        .HasColumnType("int")
                        .HasColumnName("LectionsMaxTime");

                    b.Property<int>("OtherTeachingClassesMaxTime")
                        .HasColumnType("int")
                        .HasColumnName("OtherTeachingClassesMaxTime");

                    b.Property<int?>("ParentContractID")
                        .HasColumnType("int")
                        .HasColumnName("ParentContractID");

                    b.Property<DateTime>("PeriodEnd")
                        .HasColumnType("datetime2")
                        .HasColumnName("PeriodEnd");

                    b.Property<DateTime>("PeriodStart")
                        .HasColumnType("datetime2")
                        .HasColumnName("PeriodStart");

                    b.Property<int>("PlasticPosesDemonstrationMaxTime")
                        .HasColumnType("int")
                        .HasColumnName("PlasticPosesDemonstrationMaxTime");

                    b.Property<int>("PracticalClassesMaxTime")
                        .HasColumnType("int")
                        .HasColumnName("PracticalClassesMaxTime");

                    b.Property<int>("SECMaxTime")
                        .HasColumnType("int")
                        .HasColumnName("SECMaxTime");

                    b.Property<int>("TestingEscortMaxTime")
                        .HasColumnType("int")
                        .HasColumnName("TestingEscortMaxTime");

                    b.Property<int>("TestsAndReferatsMaxTime")
                        .HasColumnType("int")
                        .HasColumnName("TestsAndReferatsMaxTime");

                    b.Property<int>("UserID")
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    b.HasKey("ID");

                    b.HasIndex("ConfirmedByUserID");

                    b.HasIndex("DepartmentID");

                    b.HasIndex("ParentContractID")
                        .IsUnique()
                        .HasFilter("[ParentContractID] IS NOT NULL");

                    b.HasIndex("UserID");

                    b.ToTable("Contracts");
                });

            modelBuilder.Entity("Database.Entities.Department", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("Name");

                    b.HasKey("ID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("Database.Entities.MonthReport", b =>
                {
                    b.Property<int>("Month")
                        .HasColumnType("int")
                        .HasColumnName("Month");

                    b.Property<int>("Year")
                        .HasColumnType("int")
                        .HasColumnName("Year");

                    b.Property<int>("ContractID")
                        .HasColumnType("int");

                    b.Property<int>("ConsultationsTime")
                        .HasColumnType("int")
                        .HasColumnName("ConsultationsTime");

                    b.Property<int>("CourseProjectsTime")
                        .HasColumnType("int")
                        .HasColumnName("CourseProjectsTime");

                    b.Property<int>("CreditsTime")
                        .HasColumnType("int")
                        .HasColumnName("CreditsTime");

                    b.Property<int>("DiplomasReviewsTime")
                        .HasColumnType("int")
                        .HasColumnName("DiplomasReviewsTime");

                    b.Property<int>("DiplomasTime")
                        .HasColumnType("int")
                        .HasColumnName("DiplomasTime");

                    b.Property<int>("ExamsTime")
                        .HasColumnType("int")
                        .HasColumnName("ExamsTime");

                    b.Property<int>("GraduatesAcademicWorkTime")
                        .HasColumnType("int")
                        .HasColumnName("GraduatesAcademicWorkTime");

                    b.Property<int>("GraduatesManagementTime")
                        .HasColumnType("int")
                        .HasColumnName("GraduatesManagementTime");

                    b.Property<int>("InternshipsTime")
                        .HasColumnType("int")
                        .HasColumnName("InternshipsTime");

                    b.Property<int>("InterviewsTime")
                        .HasColumnType("int")
                        .HasColumnName("InterviewsTime");

                    b.Property<int>("LaboratoryClassesTime")
                        .HasColumnType("int")
                        .HasColumnName("LaboratoryClassesTime");

                    b.Property<int>("LectionsTime")
                        .HasColumnType("int")
                        .HasColumnName("LectionsTime");

                    b.Property<int>("OtherTeachingClassesTime")
                        .HasColumnType("int")
                        .HasColumnName("OtherTeachingClassesTime");

                    b.Property<int>("PlasticPosesDemonstrationTime")
                        .HasColumnType("int")
                        .HasColumnName("PlasticPosesDemonstrationTime");

                    b.Property<int>("PracticalClassesTime")
                        .HasColumnType("int")
                        .HasColumnName("PracticalClassesTime");

                    b.Property<int>("SECTime")
                        .HasColumnType("int")
                        .HasColumnName("SECTime");

                    b.Property<int>("TestingEscortTime")
                        .HasColumnType("int")
                        .HasColumnName("TestingEscortTime");

                    b.Property<int>("TestsAndReferatsTime")
                        .HasColumnType("int")
                        .HasColumnName("TestsAndReferatsTime");

                    b.HasKey("Month", "Year", "ContractID");

                    b.HasIndex("ContractID");

                    b.ToTable("MonthReports");
                });

            modelBuilder.Entity("Database.Entities.Role", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("Name");

                    b.HasKey("ID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            ID = 3,
                            Name = "Admin"
                        },
                        new
                        {
                            ID = 1,
                            Name = "SuperAdmin"
                        },
                        new
                        {
                            ID = 2,
                            Name = "User"
                        });
                });

            modelBuilder.Entity("Database.Entities.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("Login");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("Name");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("NVARCHAR")
                        .HasColumnName("PasswordHash");

                    b.Property<string>("Patronymic")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("Patronymic");

                    b.Property<int>("RoleId")
                        .HasColumnType("int")
                        .HasColumnName("RoleID");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("Surname");

                    b.HasKey("ID");

                    b.HasIndex("Login")
                        .IsUnique();

                    b.HasIndex("RoleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Database.Entities.Contract", b =>
                {
                    b.HasOne("Database.Entities.User", "ConfirmedByUser")
                        .WithMany("ConfirmedContracts")
                        .HasForeignKey("ConfirmedByUserID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .HasConstraintName("FK_Contract_Confirmed_By_User");

                    b.HasOne("Database.Entities.Department", "Department")
                        .WithMany("Contracts")
                        .HasForeignKey("DepartmentID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Contracts_Departments");

                    b.HasOne("Database.Entities.Contract", "ParentContract")
                        .WithOne("ChildContract")
                        .HasForeignKey("Database.Entities.Contract", "ParentContractID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .HasConstraintName("FK_Contrcat_Contract");

                    b.HasOne("Database.Entities.User", "User")
                        .WithMany("Contracts")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Contracts_Users");

                    b.Navigation("ConfirmedByUser");

                    b.Navigation("Department");

                    b.Navigation("ParentContract");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Database.Entities.MonthReport", b =>
                {
                    b.HasOne("Database.Entities.Contract", "Contract")
                        .WithMany("MonthReports")
                        .HasForeignKey("ContractID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Contract");
                });

            modelBuilder.Entity("Database.Entities.User", b =>
                {
                    b.HasOne("Database.Entities.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Roles_Users");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Database.Entities.Contract", b =>
                {
                    b.Navigation("ChildContract");

                    b.Navigation("MonthReports");
                });

            modelBuilder.Entity("Database.Entities.Department", b =>
                {
                    b.Navigation("Contracts");
                });

            modelBuilder.Entity("Database.Entities.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("Database.Entities.User", b =>
                {
                    b.Navigation("ConfirmedContracts");

                    b.Navigation("Contracts");
                });
#pragma warning restore 612, 618
        }
    }
}
