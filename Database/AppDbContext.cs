using Database.Constants;
using Database.Entities;
using Database.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Database
{
    public class AppDbContext : DbContext, IAppDBContext
    {

        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Environment.GetEnvironmentVariable("DiplomaDatabaseConnectionString") ?? throw new Exception($"DiplomaDatabaseConnectionString not found'"), new MySqlServerVersion(new Version(8,0,33)));
            //optionsBuilder.UseMySql(Environment.GetEnvironmentVariable("DiplomaLocalMySQLConnectionString") ?? throw new Exception($"DiplomaLocalMySQLConnectionString not found'"), new MySqlServerVersion(new Version(8, 0, 33)));
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

            modelBuilder.Entity<ContractTypePriceAssignment>(entity =>
            {
                entity.Property(x => x.ObjectIdentifier).HasColumnName("ObjectIdentifier");
                entity.Property(e => e.Value).HasColumnName("Value");
                entity.Property(e => e.AssignmentDate).HasColumnName("AssignmentDate");
                entity.HasKey(e => new { e.AssignmentDate, e.ObjectIdentifier });
            });

            modelBuilder.Entity<AcademicDegreePriceAssignment>(entity =>
            {
                entity.Property(x => x.ObjectIdentifier).HasColumnName("ObjectIdentifier");
                entity.Property(e => e.Value).HasColumnName("Value");
                entity.Property(e => e.AssignmentDate).HasColumnName("AssignmentDate");
                entity.HasKey(e => new { e.AssignmentDate, e.ObjectIdentifier });

            });

            modelBuilder.Entity<UserAcademicDegreeAssignment>(entity =>
            {
                entity.Property(x => x.ObjectIdentifier).HasColumnName("ObjectIdentifier");
                entity.Property(e => e.Value).HasColumnName("Value");
                entity.Property(e => e.AssignmentDate).HasColumnName("AssignmentDate");
                entity.HasKey(e => new { e.AssignmentDate, e.ObjectIdentifier });
                entity.HasOne(e => e.ValueRef).WithMany(e => e.UserAssignations).HasForeignKey(e => e.Value);
            });

            modelBuilder.Entity<ContractType>(entity =>
            {
                entity.Property(x => x.ID).HasColumnName("ID").UseMySqlIdentityColumn();
                entity.Property(e => e.Name).HasColumnName("Name");
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasMany(e => e.Assignments)
                .WithOne(a => a.ObjectRef)
                .HasForeignKey(a => a.ObjectIdentifier)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ContractType_ContractTypeValueAssignment");
            });

            modelBuilder.Entity<AcademicDegree>(entity =>
            {
                entity.Property(x => x.ID).HasColumnName("ID").UseMySqlIdentityColumn();
                entity.Property(e => e.Name).HasColumnName("Name");
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasMany(e => e.Assignments)
                .WithOne(a => a.ObjectRef)
                .HasForeignKey(a => a.ObjectIdentifier)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_AcademicDegree_AcademicDegreeValueAssignment");
            });

            modelBuilder.Entity<ContractLinkingPart>(entity =>
            {
                entity.Property(x => x.ID).HasColumnName("ID").UseMySqlIdentityColumn();
                entity.Property(x => x.SourceContractID).HasColumnName("SourceContractID");
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
                entity.Property(e => e.NSP).HasComputedColumnSql(@"TRIM(concat(u.Name, ' ', u.Surname, ' ', u.Patronymic))", stored: false);
                entity.HasOne(e => e.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Roles_Users");
                entity.HasMany(e => e.Assignments)
                .WithOne(a => a.ObjectRef)
                .HasForeignKey(a => a.ObjectIdentifier)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_User_UserAcademicDegreeAssignment");
            });

            modelBuilder.Entity<Contract>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("ID").UseMySqlIdentityColumn();
                entity.Property(e => e.UserID).HasColumnName("UserID");
                entity.Property(e => e.ParentContractID).IsRequired(false).HasColumnName("ParentContractID");
                entity.Property(e => e.ContractIdentifier).HasColumnName("ContractIdentifier");
                entity.HasIndex(e => e.ContractIdentifier).IsUnique();
                entity.Property(e => e.AssignmentDate).HasColumnName("AssignmentDate");
                entity.Property(e => e.ConclusionDate).HasColumnName("ConclusionDate");
                entity.Property(e => e.PeriodStart).HasColumnName("PeriodStart");
                entity.Property(e => e.PeriodEnd).HasColumnName("PeriodEnd");
                entity.Property(e => e.ConfirmedByUserID).IsRequired(false).HasColumnName("ConfirmedByUserID");
                entity.Property(e => e.LinkingPartID).IsRequired(false).HasColumnName("LinkingPartID");
                entity.Property(e => e.IsConfirmed).HasComputedColumnSql(@"ConfirmedByUserID is not null", stored: false);
                entity.HasOne(e => e.LinkingPart)
                .WithMany(a => a.Assignments)
                .HasForeignKey(a => a.LinkingPartID)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Contract_LinkingPart");
                entity.HasOne(e => e.ConfirmedByUser)
                .WithMany(e => e.ConfirmedContracts)
                .IsRequired(false)
                .HasForeignKey(e => e.ConfirmedByUserID)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Contract_Confirmed_By_User");
                entity.HasOne(e => e.ParentContract)
                .WithMany(e => e.ChildContracts)
                .IsRequired(false)
                .HasForeignKey(e => e.ParentContractID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Contrcat_Contract_Parent");
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
                entity.HasOne(e => e.ContractType)
                .WithMany(a => a.Contracts)
                .HasForeignKey(a => a.ContractTypeID)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Contract_ContractType");
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
                entity.HasKey(e => new { e.Month, e.Year, e.LinkingPartID });
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
                entity.Property(e => e.BlockedByUserID).IsRequired(false).HasColumnName("BlockedByUserID");
                entity.Property(e => e.MontYearAsDate).HasComputedColumnSql(@"STR_TO_DATE(Concat('1',',', Month,',', Year),'%d,%m,%Y')", stored: false);
                entity.Property(e => e.IsBlocked).HasComputedColumnSql(@"BlockedByUserID is not null", stored: false);
                entity.HasOne(e => e.BlockedByUser)
                .WithMany(e => e.BlockedReports)
                .HasForeignKey(e => e.BlockedByUserID)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_MonthReport_Blocked_By_User");
            });

            modelBuilder.Entity<Role>().HasData(new Role() { ID = IncludeModels.RolesNavigation.SuperAdminRoleID, Name = IncludeModels.RolesNavigation.SuperAdminRoleName });
            modelBuilder.Entity<Role>().HasData(new Role() { ID = IncludeModels.RolesNavigation.OrdinaryUserRoleID, Name = IncludeModels.RolesNavigation.OrdinaryUserRoleName });
            modelBuilder.Entity<Role>().HasData(new Role() { ID = IncludeModels.RolesNavigation.AdminRoleID, Name = IncludeModels.RolesNavigation.AdminRoleName });

            modelBuilder.Entity<AcademicDegree>().HasData(new AcademicDegree { ID = 1, Name = "Doctor" });
            modelBuilder.Entity<AcademicDegree>().HasData(new AcademicDegree { ID = 2, Name = "Professor" });

            modelBuilder.Entity<AcademicDegreePriceAssignment>().HasData(new AcademicDegreePriceAssignment { ObjectIdentifier = 1, AssignmentDate = new DateTime(2023, 5, 1), Value = 12 });
            modelBuilder.Entity<AcademicDegreePriceAssignment>().HasData(new AcademicDegreePriceAssignment { ObjectIdentifier = 2, AssignmentDate = new DateTime(2023, 5, 1), Value = 10 });

            modelBuilder.Entity<ContractType>().HasData(new ContractType { ID = 1, Name = "Ordinary" });
            modelBuilder.Entity<ContractType>().HasData(new ContractType { ID = 2, Name = "Advanced" });

            modelBuilder.Entity<ContractTypePriceAssignment>().HasData(new ContractTypePriceAssignment { ObjectIdentifier = 1, AssignmentDate = new DateTime(2023, 5, 1), Value = 12 });
            modelBuilder.Entity<ContractTypePriceAssignment>().HasData(new ContractTypePriceAssignment { ObjectIdentifier = 2, AssignmentDate = new DateTime(2023, 5, 1), Value = 10 });

            modelBuilder.Entity<User>().HasData(new User() { ID = 1, Name = "admin", Surname = "admin", Patronymic = "admin", PasswordHash = "!#/)zW��C�J\u000eJ�\u001f�", RoleId = IncludeModels.RolesNavigation.SuperAdminRoleID, Login = "admin" });
            modelBuilder.Entity<User>().HasData(new User() { ID = 2, Name = "name1", Surname = "name1", Patronymic = "name1", PasswordHash = "!#/)zW��C�J\u000eJ�\u001f�", RoleId = IncludeModels.RolesNavigation.AdminRoleID, Login = "name1" });
            modelBuilder.Entity<User>().HasData(new User() { ID = 3, Name = "name2", Surname = "name2", Patronymic = "name2", PasswordHash = "!#/)zW��C�J\u000eJ�\u001f�", RoleId = IncludeModels.RolesNavigation.OrdinaryUserRoleID, Login = "name2" });

            modelBuilder.Entity<UserAcademicDegreeAssignment>().HasData(new UserAcademicDegreeAssignment { ObjectIdentifier = 1, AssignmentDate = new DateTime(2023, 5, 1), Value = 1 });

            modelBuilder.Entity<Department>().HasData(new Department { ID = 1, Name = "FITR" });
            modelBuilder.Entity<Department>().HasData(new Department { ID = 2, Name = "FTUG" });
        }
    }
}