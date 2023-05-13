﻿// <auto-generated />
using System;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Database.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Database.Entities.AcademicDegree", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID")
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("Name");

                    b.HasKey("ID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("AcademicDegrees");

                    b.HasData(
                        new
                        {
                            ID = 1,
                            Name = "Doctor"
                        },
                        new
                        {
                            ID = 2,
                            Name = "Professor"
                        });
                });

            modelBuilder.Entity("Database.Entities.AcademicDegreePriceAssignation", b =>
                {
                    b.Property<DateTime>("AssignmentDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("AssignationDate");

                    b.Property<int>("ObjectIdentifier")
                        .HasColumnType("int")
                        .HasColumnName("ObjectIdentifier");

                    b.Property<double>("Value")
                        .HasColumnType("double")
                        .HasColumnName("Value");

                    b.HasKey("AssignmentDate", "ObjectIdentifier");

                    b.HasIndex("ObjectIdentifier");

                    b.ToTable("AcademicDegreePriceAssignations");

                    b.HasData(
                        new
                        {
                            AssignmentDate = new DateTime(2023, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ObjectIdentifier = 1,
                            Value = 12.0
                        },
                        new
                        {
                            AssignmentDate = new DateTime(2023, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ObjectIdentifier = 2,
                            Value = 10.0
                        });
                });

            modelBuilder.Entity("Database.Entities.Contract", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID")
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ConfirmedByUserID")
                        .HasColumnType("int")
                        .HasColumnName("ConfirmedByUserID");

                    b.Property<double>("ConsultationsMaxTime")
                        .HasColumnType("double")
                        .HasColumnName("ConsultationsMaxTime");

                    b.Property<string>("ContractIdentifier")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("ContractIdentifier");

                    b.Property<int>("ContractTypeID")
                        .HasColumnType("int");

                    b.Property<double>("CourseProjectsMaxTime")
                        .HasColumnType("double")
                        .HasColumnName("CourseProjectsMaxTime");

                    b.Property<double>("CreditsMaxTime")
                        .HasColumnType("double")
                        .HasColumnName("CreditsMaxTime");

                    b.Property<int>("DepartmentID")
                        .HasColumnType("int");

                    b.Property<double>("DiplomasMaxTime")
                        .HasColumnType("double")
                        .HasColumnName("DiplomasMaxTime");

                    b.Property<double>("DiplomasReviewsMaxTime")
                        .HasColumnType("double")
                        .HasColumnName("DiplomasReviewsMaxTime");

                    b.Property<double>("ExamsMaxTime")
                        .HasColumnType("double")
                        .HasColumnName("ExamsMaxTime");

                    b.Property<double>("GraduatesAcademicWorkMaxTime")
                        .HasColumnType("double")
                        .HasColumnName("GraduatesAcademicWorkMaxTime");

                    b.Property<double>("GraduatesManagementMaxTime")
                        .HasColumnType("double")
                        .HasColumnName("GraduatesManagementMaxTime");

                    b.Property<double>("InternshipsMaxTime")
                        .HasColumnType("double")
                        .HasColumnName("InternshipsMaxTime");

                    b.Property<double>("InterviewsMaxTime")
                        .HasColumnType("double")
                        .HasColumnName("InterviewsMaxTime");

                    b.Property<double>("LaboratoryClassesMaxTime")
                        .HasColumnType("double")
                        .HasColumnName("LaboratoryClassesMaxTime");

                    b.Property<double>("LectionsMaxTime")
                        .HasColumnType("double")
                        .HasColumnName("LectionsMaxTime");

                    b.Property<double>("OtherTeachingClassesMaxTime")
                        .HasColumnType("double")
                        .HasColumnName("OtherTeachingClassesMaxTime");

                    b.Property<int?>("ParentContractID")
                        .HasColumnType("int")
                        .HasColumnName("ParentContractID");

                    b.Property<DateTime>("PeriodEnd")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("PeriodEnd");

                    b.Property<DateTime>("PeriodStart")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("PeriodStart");

                    b.Property<double>("PlasticPosesDemonstrationMaxTime")
                        .HasColumnType("double")
                        .HasColumnName("PlasticPosesDemonstrationMaxTime");

                    b.Property<double>("PracticalClassesMaxTime")
                        .HasColumnType("double")
                        .HasColumnName("PracticalClassesMaxTime");

                    b.Property<double>("SECMaxTime")
                        .HasColumnType("double")
                        .HasColumnName("SECMaxTime");

                    b.Property<double>("TestingEscortMaxTime")
                        .HasColumnType("double")
                        .HasColumnName("TestingEscortMaxTime");

                    b.Property<double>("TestsAndReferatsMaxTime")
                        .HasColumnType("double")
                        .HasColumnName("TestsAndReferatsMaxTime");

                    b.Property<int>("UserID")
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    b.HasKey("ID");

                    b.HasIndex("ConfirmedByUserID");

                    b.HasIndex("ContractIdentifier")
                        .IsUnique();

                    b.HasIndex("ContractTypeID");

                    b.HasIndex("DepartmentID");

                    b.HasIndex("ParentContractID")
                        .IsUnique();

                    b.HasIndex("UserID");

                    b.ToTable("Contracts");
                });

            modelBuilder.Entity("Database.Entities.ContractType", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID")
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("Name");

                    b.HasKey("ID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("ContractTypes");

                    b.HasData(
                        new
                        {
                            ID = 1,
                            Name = "Ordinary"
                        },
                        new
                        {
                            ID = 2,
                            Name = "Advanced"
                        });
                });

            modelBuilder.Entity("Database.Entities.ContractTypePriceAssignment", b =>
                {
                    b.Property<DateTime>("AssignmentDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("AssignationDate");

                    b.Property<int>("ObjectIdentifier")
                        .HasColumnType("int")
                        .HasColumnName("ObjectIdentifier");

                    b.Property<double>("Value")
                        .HasColumnType("double")
                        .HasColumnName("Value");

                    b.HasKey("AssignmentDate", "ObjectIdentifier");

                    b.HasIndex("ObjectIdentifier");

                    b.ToTable("ContractTypePriceAssignments");

                    b.HasData(
                        new
                        {
                            AssignmentDate = new DateTime(2023, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ObjectIdentifier = 1,
                            Value = 12.0
                        },
                        new
                        {
                            AssignmentDate = new DateTime(2023, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ObjectIdentifier = 2,
                            Value = 10.0
                        });
                });

            modelBuilder.Entity("Database.Entities.Department", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID")
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("Name");

                    b.HasKey("ID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Departments");

                    b.HasData(
                        new
                        {
                            ID = 1,
                            Name = "FITR"
                        },
                        new
                        {
                            ID = 2,
                            Name = "FTUG"
                        });
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

                    b.Property<double>("ConsultationsTime")
                        .HasColumnType("double")
                        .HasColumnName("ConsultationsTime");

                    b.Property<double>("CourseProjectsTime")
                        .HasColumnType("double")
                        .HasColumnName("CourseProjectsTime");

                    b.Property<double>("CreditsTime")
                        .HasColumnType("double")
                        .HasColumnName("CreditsTime");

                    b.Property<double>("DiplomasReviewsTime")
                        .HasColumnType("double")
                        .HasColumnName("DiplomasReviewsTime");

                    b.Property<double>("DiplomasTime")
                        .HasColumnType("double")
                        .HasColumnName("DiplomasTime");

                    b.Property<double>("ExamsTime")
                        .HasColumnType("double")
                        .HasColumnName("ExamsTime");

                    b.Property<double>("GraduatesAcademicWorkTime")
                        .HasColumnType("double")
                        .HasColumnName("GraduatesAcademicWorkTime");

                    b.Property<double>("GraduatesManagementTime")
                        .HasColumnType("double")
                        .HasColumnName("GraduatesManagementTime");

                    b.Property<double>("InternshipsTime")
                        .HasColumnType("double")
                        .HasColumnName("InternshipsTime");

                    b.Property<double>("InterviewsTime")
                        .HasColumnType("double")
                        .HasColumnName("InterviewsTime");

                    b.Property<double>("LaboratoryClassesTime")
                        .HasColumnType("double")
                        .HasColumnName("LaboratoryClassesTime");

                    b.Property<double>("LectionsTime")
                        .HasColumnType("double")
                        .HasColumnName("LectionsTime");

                    b.Property<double>("OtherTeachingClassesTime")
                        .HasColumnType("double")
                        .HasColumnName("OtherTeachingClassesTime");

                    b.Property<double>("PlasticPosesDemonstrationTime")
                        .HasColumnType("double")
                        .HasColumnName("PlasticPosesDemonstrationTime");

                    b.Property<double>("PracticalClassesTime")
                        .HasColumnType("double")
                        .HasColumnName("PracticalClassesTime");

                    b.Property<double>("SECTime")
                        .HasColumnType("double")
                        .HasColumnName("SECTime");

                    b.Property<double>("TestingEscortTime")
                        .HasColumnType("double")
                        .HasColumnName("TestingEscortTime");

                    b.Property<double>("TestsAndReferatsTime")
                        .HasColumnType("double")
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
                        .HasColumnName("ID")
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("Name");

                    b.HasKey("ID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            ID = 1,
                            Name = "SuperAdmin"
                        },
                        new
                        {
                            ID = 2,
                            Name = "User"
                        },
                        new
                        {
                            ID = 3,
                            Name = "Admin"
                        });
                });

            modelBuilder.Entity("Database.Entities.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID")
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ConfirmedByUserID")
                        .HasColumnType("int")
                        .HasColumnName("ConfirmedByUserID");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("Login");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("Name");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("NVARCHAR")
                        .HasColumnName("PasswordHash");

                    b.Property<string>("Patronymic")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("Patronymic");

                    b.Property<int>("RoleId")
                        .HasColumnType("int")
                        .HasColumnName("RoleID");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("Surname");

                    b.HasKey("ID");

                    b.HasIndex("ConfirmedByUserID");

                    b.HasIndex("Login")
                        .IsUnique();

                    b.HasIndex("RoleId");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            ID = 1,
                            ConfirmedByUserID = 1,
                            Login = "admin",
                            Name = "admin",
                            PasswordHash = "!#/)zW��C�JJ��",
                            Patronymic = "admin",
                            RoleId = 1,
                            Surname = "admin"
                        });
                });

            modelBuilder.Entity("Database.Entities.UserAcademicDegreeAssignament", b =>
                {
                    b.Property<DateTime>("AssignmentDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("AssignationDate");

                    b.Property<int>("ObjectIdentifier")
                        .HasColumnType("int")
                        .HasColumnName("ObjectIdentifier");

                    b.Property<int>("Value")
                        .HasColumnType("int")
                        .HasColumnName("Value");

                    b.HasKey("AssignmentDate", "ObjectIdentifier");

                    b.HasIndex("ObjectIdentifier");

                    b.HasIndex("Value");

                    b.ToTable("UserAcademicDegreeAssignaments");

                    b.HasData(
                        new
                        {
                            AssignmentDate = new DateTime(2023, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ObjectIdentifier = 1,
                            Value = 1
                        });
                });

            modelBuilder.Entity("Database.Entities.AcademicDegreePriceAssignation", b =>
                {
                    b.HasOne("Database.Entities.AcademicDegree", "ObjectRef")
                        .WithMany("Assignments")
                        .HasForeignKey("ObjectIdentifier")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_AcademicDegree_AcademicDegreeValueAssignation");

                    b.Navigation("ObjectRef");
                });

            modelBuilder.Entity("Database.Entities.Contract", b =>
                {
                    b.HasOne("Database.Entities.User", "ConfirmedByUser")
                        .WithMany("ConfirmedContracts")
                        .HasForeignKey("ConfirmedByUserID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .HasConstraintName("FK_Contract_Confirmed_By_User");

                    b.HasOne("Database.Entities.ContractType", "ContractType")
                        .WithMany("Contracts")
                        .HasForeignKey("ContractTypeID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired()
                        .HasConstraintName("FK_Contract_ContractType");

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

                    b.Navigation("ContractType");

                    b.Navigation("Department");

                    b.Navigation("ParentContract");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Database.Entities.ContractTypePriceAssignment", b =>
                {
                    b.HasOne("Database.Entities.ContractType", "ObjectRef")
                        .WithMany("Assignments")
                        .HasForeignKey("ObjectIdentifier")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_ContractType_ContractTypeValueAssignation");

                    b.Navigation("ObjectRef");
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
                    b.HasOne("Database.Entities.User", "ConfirmedByUser")
                        .WithMany("ConfirmedUsers")
                        .HasForeignKey("ConfirmedByUserID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .HasConstraintName("FK_User_Confirmed_By_User");

                    b.HasOne("Database.Entities.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Roles_Users");

                    b.Navigation("ConfirmedByUser");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Database.Entities.UserAcademicDegreeAssignament", b =>
                {
                    b.HasOne("Database.Entities.User", "ObjectRef")
                        .WithMany("Assignments")
                        .HasForeignKey("ObjectIdentifier")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_User_UserAcademicDegreeAssignation");

                    b.HasOne("Database.Entities.AcademicDegree", "ValueRef")
                        .WithMany("UserAssignations")
                        .HasForeignKey("Value")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ObjectRef");

                    b.Navigation("ValueRef");
                });

            modelBuilder.Entity("Database.Entities.AcademicDegree", b =>
                {
                    b.Navigation("Assignments");

                    b.Navigation("UserAssignations");
                });

            modelBuilder.Entity("Database.Entities.Contract", b =>
                {
                    b.Navigation("ChildContract");

                    b.Navigation("MonthReports");
                });

            modelBuilder.Entity("Database.Entities.ContractType", b =>
                {
                    b.Navigation("Assignments");

                    b.Navigation("Contracts");
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
                    b.Navigation("Assignments");

                    b.Navigation("ConfirmedContracts");

                    b.Navigation("ConfirmedUsers");

                    b.Navigation("Contracts");
                });
#pragma warning restore 612, 618
        }
    }
}
