using Data.Constants;
using Database.Entities;
using Database.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Database
{
    public class AppDbContext : DbContext, IAppDBContext
    {
        public virtual DbSet<Contract> Contracts { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<MonthReport> MonthReports { get; set; } = null!;
        public virtual DbSet<Department> Departments { get; set; } = null!;

        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            optionsBuilder.UseMySql("server=icrafts.beget.tech;user=icrafts_test;password=prB%cnJ5;database=icrafts_test;", new MySqlServerVersion(new Version(8,0,33)));
            //optionsBuilder.UseMySql(Environment.GetEnvironmentVariable("DiplomaDatabaseConnectionString") ?? throw new Exception($"DiplomaDatabaseConnectionString not found'"), new MySqlServerVersion(new Version(8,0,33)));
            //optionsBuilder.UseMySql(Environment.GetEnvironmentVariable("DiplomaLocalMySQLConnectionString") ?? throw new Exception($"DiplomaLocalMySQLConnectionString not found'"), new MySqlServerVersion(new Version(8,0,33)));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(x => x.ID).HasColumnName("ID").UseMySqlIdentityColumn();
                entity.Property(e => e.Name).HasColumnName("Name");
                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.Property(x => x.ID).HasColumnName("ID").UseMySqlIdentityColumn();
                entity.Property(e => e.Name).HasColumnName("Name");
                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => new { e.Login }).IsUnique();
                entity.Property(e => e.ID).HasColumnName("ID").UseMySqlIdentityColumn();
                entity.Property(e => e.RoleId).HasColumnName("RoleID");
                entity.Property(e => e.Name).HasColumnName("Name").IsRequired().HasMaxLength(50);
                entity.Property(e => e.Surname).HasColumnName("Surname").IsRequired().HasMaxLength(50);
                entity.Property(e => e.Patronymic).HasColumnName("Patronymic").HasMaxLength(50);
                entity.Property(e => e.PasswordHash).HasColumnName("PasswordHash").HasColumnType("NVARCHAR").HasMaxLength(200).IsRequired();
                entity.Property(e => e.Login).HasColumnName("Login").IsRequired().HasMaxLength(50);
                entity.HasOne(e => e.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Roles_Users");
                entity.Property(e => e.ConfirmedByUserID).HasColumnName("ConfirmedByUserID");
                entity.HasOne(e => e.ConfirmedByUser)
                .WithMany(e => e.ConfirmedUsers)
                .HasForeignKey(e => e.ConfirmedByUserID)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_User_Confirmed_By_User");
            });

            modelBuilder.Entity<Contract>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("ID").UseMySqlIdentityColumn();
                entity.Property(e => e.UserID).HasColumnName("UserID");
                entity.Property(e => e.ParentContractID).HasColumnName("ParentContractID");
                entity.HasIndex(e => e.ParentContractID).IsUnique();
                entity.Property(e => e.ContractIdentifier).HasColumnName("ContractIdentifier");
                entity.HasIndex(e => e.ContractIdentifier).IsUnique();
                entity.Property(e => e.PeriodStart).HasColumnName("PeriodStart");
                entity.Property(e => e.PeriodEnd).HasColumnName("PeriodEnd");
                entity.Property(e => e.ConfirmedByUserID).HasColumnName("ConfirmedByUserID");
                entity.HasOne(e => e.ConfirmedByUser)
                .WithMany(e => e.ConfirmedContracts)
                .HasForeignKey(e => e.ConfirmedByUserID)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Contract_Confirmed_By_User");
                entity.HasOne(e => e.ParentContract)
                .WithOne(e => e.ChildContract)
                .HasForeignKey<Contract>(e => e.ParentContractID)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Contrcat_Contract");
                entity.HasOne(e => e.User)
                .WithMany(u => u.Contracts)
                .HasForeignKey(u => u.UserID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Contracts_Users"); 
                entity.HasOne(e => e.Department)
                .WithMany(u => u.Contracts)
                .HasForeignKey(u => u.DepartmentID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Contracts_Departments");
                entity.Property(e => e.LectionsMaxTime).HasColumnName("LectionsMaxTime");
                entity.Property(e => e.PracticalClassesMaxTime).HasColumnName("PracticalClassesMaxTime");
                entity.Property(e => e.LaboratoryClassesMaxTime).HasColumnName("LaboratoryClassesMaxTime");
                entity.Property(e => e.ConsultationsMaxTime).HasColumnName("ConsultationsMaxTime");
                entity.Property(e => e.OtherTeachingClassesMaxTime).HasColumnName("OtherTeachingClassesMaxTime");
                entity.Property(e => e.CreditsMaxTime).HasColumnName("CreditsMaxTime");
                entity.Property(e => e.ExamsMaxTime).HasColumnName("ExamsMaxTime");
                entity.Property(e => e.CourseProjectsMaxTime).HasColumnName("CourseProjectsMaxTime");
                entity.Property(e => e.InterviewsMaxTime).HasColumnName("InterviewsMaxTime");
                entity.Property(e => e.TestsAndReferatsMaxTime).HasColumnName("TestsAndReferatsMaxTime");
                entity.Property(e => e.InternshipsMaxTime).HasColumnName("InternshipsMaxTime");
                entity.Property(e => e.DiplomasMaxTime).HasColumnName("DiplomasMaxTime");
                entity.Property(e => e.DiplomasReviewsMaxTime).HasColumnName("DiplomasReviewsMaxTime");
                entity.Property(e => e.SECMaxTime).HasColumnName("SECMaxTime");
                entity.Property(e => e.GraduatesManagementMaxTime).HasColumnName("GraduatesManagementMaxTime");
                entity.Property(e => e.GraduatesAcademicWorkMaxTime).HasColumnName("GraduatesAcademicWorkMaxTime");
                entity.Property(e => e.PlasticPosesDemonstrationMaxTime).HasColumnName("PlasticPosesDemonstrationMaxTime");
                entity.Property(e => e.TestingEscortMaxTime).HasColumnName("TestingEscortMaxTime");
            });

            modelBuilder.Entity<MonthReport>(entity =>
            {
                entity.Property(e => e.Month).HasColumnName("Month");
                entity.Property(e => e.Year).HasColumnName("Year");
                entity.HasKey(e => new { e.Month, e.Year, e.ContractID });
                entity.Property(e => e.LectionsTime).HasColumnName("LectionsTime");
                entity.Property(e => e.PracticalClassesTime).HasColumnName("PracticalClassesTime");
                entity.Property(e => e.LaboratoryClassesTime).HasColumnName("LaboratoryClassesTime");
                entity.Property(e => e.ConsultationsTime).HasColumnName("ConsultationsTime");
                entity.Property(e => e.OtherTeachingClassesTime).HasColumnName("OtherTeachingClassesTime");
                entity.Property(e => e.CreditsTime).HasColumnName("CreditsTime");
                entity.Property(e => e.ExamsTime).HasColumnName("ExamsTime");
                entity.Property(e => e.CourseProjectsTime).HasColumnName("CourseProjectsTime");
                entity.Property(e => e.InterviewsTime).HasColumnName("InterviewsTime");
                entity.Property(e => e.TestsAndReferatsTime).HasColumnName("TestsAndReferatsTime");
                entity.Property(e => e.InternshipsTime).HasColumnName("InternshipsTime");
                entity.Property(e => e.DiplomasTime).HasColumnName("DiplomasTime");
                entity.Property(e => e.DiplomasReviewsTime).HasColumnName("DiplomasReviewsTime");
                entity.Property(e => e.SECTime).HasColumnName("SECTime");
                entity.Property(e => e.GraduatesManagementTime).HasColumnName("GraduatesManagementTime");
                entity.Property(e => e.GraduatesAcademicWorkTime).HasColumnName("GraduatesAcademicWorkTime");
                entity.Property(e => e.PlasticPosesDemonstrationTime).HasColumnName("PlasticPosesDemonstrationTime");
                entity.Property(e => e.TestingEscortTime).HasColumnName("TestingEscortTime");
            });

            modelBuilder.Entity<Role>().HasData(new Role() { ID = IncludeModels.RolesNavigation.AdminRoleID, Name = IncludeModels.RolesNavigation.AdminRoleName});
            modelBuilder.Entity<Role>().HasData(new Role() { ID = IncludeModels.RolesNavigation.SuperAdminRoleID, Name = IncludeModels.RolesNavigation.SuperAdminRoleName });
            modelBuilder.Entity<Role>().HasData(new Role() { ID = IncludeModels.RolesNavigation.OrdinaryUserRoleID, Name = IncludeModels.RolesNavigation.OrdinaryUserRoleName });

            modelBuilder.Entity<User>().HasData(new User() { ID = 1, Name = "admin", Surname = "admin", Patronymic = "admin", PasswordHash = "!#/)zW��C�J\u000eJ�\u001f�", RoleId = IncludeModels.RolesNavigation.SuperAdminRoleID, ConfirmedByUserID = 1, Login = "admin" });
        }
    }
}